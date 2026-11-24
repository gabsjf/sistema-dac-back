# 4. Modelagem

Os diagramas abaixo refletem o código (`ampara-api/src/Ampara/Domain` e `Infrastructure/Persistence`).

## 4.1. Modelo de dados (banco PostgreSQL)

São **três tabelas**. Os valores enumerados (papel, espécie, situação…) são gravados como **texto**, para ficarem legíveis direto no banco. As chaves são `uuid`.

```mermaid
erDiagram
    Users ||--o{ AdoptionRequests : "faz"
    Animals ||--o{ AdoptionRequests : "recebe"

    Users {
        uuid Id PK
        string FullName
        string Email UK
        string Phone "opcional"
        string Role "SuperAdmin | Admin | Adopter"
        string PasswordHash
        timestamptz CreatedAt
    }

    Animals {
        uuid Id PK
        string Name
        string Species "Canine | Feline | Other"
        string Sex "Female | Male"
        string Size "Small | Medium | Large"
        int ApproximateAgeMonths
        date IntakeDate
        string Description "opcional"
        bool IsVaccinated
        bool IsNeutered
        bool IsDewormed
        string PhotoUrl "opcional"
        string Status "Available | InTreatment | Adopted"
        timestamptz CreatedAt
    }

    AdoptionRequests {
        uuid Id PK
        uuid AnimalId FK
        uuid AdopterId FK
        string Phone
        string Housing
        string Routine
        string Message "opcional"
        string Status "Pending | Approved | Rejected"
        string RejectionReason "só se recusada"
        timestamptz CreatedAt
        timestamptz DecidedAt "quando decidida"
    }
```

**Observações**

- Não há tabelas de saúde, financeiro, voluntários ou ONGs. Esses módulos estão no [roadmap](08-demo-limitacoes-e-roadmap.md#4-roadmap).
- O banco é criado e atualizado por **migration** do EF Core (`InitialCreate`). Em desenvolvimento, o sistema aplica a migration e insere dados de demonstração ao iniciar.

## 4.2. Modelo de domínio (classes)

As entidades têm **setters privados** e **métodos de fábrica**. Elas não podem ficar em estado inválido: toda regra de negócio lança `DomainException` quando violada.

```mermaid
classDiagram
    class Entity {
        +Guid Id
    }

    class User {
        +string FullName
        +string Email
        +string? Phone
        +UserRoleEnum Role
        +string PasswordHash
        +DateTimeOffset CreatedAt
        +Register(...)$ User
        +ChangeRole(role)
    }

    class Animal {
        +string Name
        +SpeciesEnum Species
        +SexEnum Sex
        +AnimalSizeEnum Size
        +int ApproximateAgeMonths
        +DateOnly IntakeDate
        +string? Description
        +bool IsVaccinated
        +bool IsNeutered
        +bool IsDewormed
        +string? PhotoUrl
        +AnimalStatusEnum Status
        +bool IsAvailable
        +Register(profile, status, now)$ Animal
        +Update(profile, status, now)
        +SetPhoto(url)
        +MarkAsAdopted()
        +RestoreAvailability()
    }

    class AdoptionRequest {
        +string Phone
        +string Housing
        +string Routine
        +string? Message
        +AdoptionStatusEnum Status
        +string? RejectionReason
        +DateTimeOffset CreatedAt
        +DateTimeOffset? DecidedAt
        +Submit(animal, adopter, details, acceptsTerms, now)$ AdoptionRequest
        +Approve(now)
        +Reject(reason, now)
        +UndoApproval()
    }

    class UserRoleEnum {
        <<enumeration>>
        SuperAdmin
        Admin
        Adopter
    }

    class AnimalStatusEnum {
        <<enumeration>>
        Available
        InTreatment
        Adopted
    }

    class AdoptionStatusEnum {
        <<enumeration>>
        Pending
        Approved
        Rejected
    }

    Entity <|-- User
    Entity <|-- Animal
    Entity <|-- AdoptionRequest
    User "1" --> "0..*" AdoptionRequest : Adopter
    Animal "1" --> "0..*" AdoptionRequest : Animal
    User --> UserRoleEnum
    Animal --> AnimalStatusEnum
    AdoptionRequest --> AdoptionStatusEnum
```

## 4.3. Máquinas de estado

### Animal

A situação **Adotado** só é alcançada por uma aprovação de adoção e só é desfeita ao desfazer essa aprovação. A edição manual alterna apenas entre *Disponível* e *Em tratamento* (RN-07).

```mermaid
stateDiagram-v2
    [*] --> Disponivel : cadastro
    [*] --> EmTratamento : cadastro

    Disponivel --> EmTratamento : edição pela equipe
    EmTratamento --> Disponivel : edição pela equipe

    Disponivel --> Adotado : aprovar adoção
    Adotado --> Disponivel : desfazer aprovação

    state "Disponível (Available)" as Disponivel
    state "Em tratamento (InTreatment)" as EmTratamento
    state "Adotado (Adopted)" as Adotado
```

> Um animal *Em tratamento* **não** pode ser adotado: ele precisa voltar a *Disponível* antes (`Animal.MarkAsAdopted`).

### Solicitação de adoção

```mermaid
stateDiagram-v2
    [*] --> Pendente : adotante envia o pedido

    Pendente --> Aprovada : equipe aprova
    Pendente --> Recusada : equipe recusa (com motivo)
    Aprovada --> Pendente : equipe desfaz a aprovação

    Recusada --> [*]

    state "Pendente (Pending)" as Pendente
    state "Aprovada (Approved)" as Aprovada
    state "Recusada (Rejected)" as Recusada
```

> Uma solicitação *Recusada* é final. O adotante pode enviar um **novo** pedido pelo mesmo animal, pois a regra de duplicidade (RN-11) só considera pedidos em análise.

## 4.4. Diagramas de sequência

### Login e uso do token

```mermaid
sequenceDiagram
    autonumber
    actor U as Usuário
    participant F as Front (Angular)
    participant A as API (AuthController)
    participant S as AuthService
    participant DB as PostgreSQL

    U->>F: informa e-mail e senha
    F->>A: POST /api/auth/login
    A->>S: LoginAsync
    S->>DB: busca usuário pelo e-mail
    DB-->>S: usuário e hash da senha
    S->>S: confere a senha com o hash (PBKDF2)
    alt senha correta
        S-->>A: AuthResponse com JWT (8 h)
        A-->>F: 200 e token
        F->>F: guarda token e usuário no sessionStorage
        F->>A: demais requisições com Authorization: Bearer token
    else e-mail ou senha incorretos
        S-->>A: UnauthorizedException
        A-->>F: 401 ProblemDetails
    end
```

### Solicitar adoção

```mermaid
sequenceDiagram
    autonumber
    actor AD as Adotante
    participant F as Front
    participant C as AdoptionsController
    participant S as AdoptionService
    participant D as Domínio (AdoptionRequest)
    participant DB as PostgreSQL

    AD->>F: preenche o formulário e aceita os termos
    F->>C: POST /api/adoptions (Bearer)
    Note over C: exige papel Adopter
    C->>S: SubmitAsync
    S->>DB: busca adotante e animal
    S->>DB: já existe pedido pendente?
    alt já existe pendente
        S-->>F: 409 Conflito
    else
        S->>D: Submit(animal, adotante, dados, termos)
        D->>D: valida disponibilidade, termos e campos
        D-->>S: solicitação Pendente
        S->>DB: grava
        S-->>F: 201 com a solicitação
    end
```

### Aprovar adoção

```mermaid
sequenceDiagram
    autonumber
    actor EQ as Equipe
    participant F as Front
    participant C as AdoptionsController
    participant S as AdoptionService
    participant D as Domínio
    participant DB as PostgreSQL

    EQ->>F: clica em "Aprovar"
    F->>C: POST /api/adoptions/{id}/approve
    Note over C: exige papel Admin ou SuperAdmin
    C->>S: ApproveAsync(id)
    S->>DB: carrega solicitação e animal
    S->>D: Approve(now)
    D->>D: exige solicitação Pendente
    D->>D: Animal.MarkAsAdopted (exige Disponível)
    D-->>S: solicitação Aprovada, animal Adotado
    S->>DB: grava as duas mudanças juntas
    S-->>F: 200 com a solicitação
    F->>F: recarrega a lista e o selo de pendências
```

### Cadastro de animal com foto

```mermaid
sequenceDiagram
    autonumber
    actor EQ as Equipe
    participant F as Front
    participant C as AnimalsController
    participant S as AnimalService
    participant ST as Armazenamento (/uploads)
    participant DB as PostgreSQL

    EQ->>F: preenche o formulário e escolhe a foto
    F->>C: POST /api/animals (dados)
    C->>S: CreateAsync
    S->>DB: grava o animal
    S-->>F: 201 com o animal criado
    F->>C: POST /api/animals/{id}/photo (multipart "file")
    C->>S: UploadPhotoAsync
    S->>S: confere tamanho (até 5 MB) e tipo pelo conteúdo
    S->>ST: salva o arquivo
    S->>DB: grava a URL da foto
    S->>ST: apaga a foto anterior, se houver
    S-->>F: 200 com o animal atualizado
```
