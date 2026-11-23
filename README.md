# 🐾 Ampara — Sistema de Gestão e Adoção Animal

O **Ampara** é um sistema web desenvolvido para auxiliar organizações de proteção animal no gerenciamento de animais e no acompanhamento de solicitações de adoção.

A aplicação permite que pessoas interessadas conheçam os animais disponíveis, realizem seu cadastro e solicitem adoções. Também disponibiliza funcionalidades administrativas para acompanhar e gerenciar essas solicitações.

O projeto utiliza uma arquitetura separada em **Frontend (Angular)**, **Backend (ASP.NET Core)** e **Banco de Dados (PostgreSQL)**.

---

## 1. Objetivo do projeto

O Ampara tem como objetivo utilizar a tecnologia para facilitar e organizar o processo de adoção responsável de animais.

Entre os principais objetivos estão:

- Divulgar animais disponíveis para adoção.
- Centralizar informações sobre os animais.
- Permitir o cadastro e a autenticação de usuários.
- Facilitar o envio de solicitações de adoção.
- Permitir que usuários acompanhem suas solicitações.
- Auxiliar administradores no gerenciamento das adoções.
- Organizar e proteger as informações armazenadas.

O sistema foi desenvolvido como projeto acadêmico, aplicando conceitos de desenvolvimento web, programação orientada a objetos, APIs REST, banco de dados e segurança.

## 2. Funcionalidades

### 2.1. Área pública

A área pública pode ser acessada sem autenticação e oferece:

- Página inicial com apresentação do projeto.
- Listagem de animais disponíveis para adoção.
- Visualização de informações individuais dos animais.
- Acesso às páginas de cadastro e login.

### 2.2. Área do usuário

Após realizar o cadastro e entrar no sistema, o usuário pode:

- Acessar funcionalidades protegidas por autenticação.
- Consultar informações de animais.
- Preencher formulários de solicitação de adoção.
- Enviar solicitações para análise.
- Acompanhar suas solicitações.

### 2.3. Área administrativa

O sistema possui controle de acesso por perfis, permitindo que usuários autorizados realizem atividades administrativas.

Entre as funcionalidades estão:

- Consultar solicitações de adoção.
- Acompanhar o andamento das solicitações.
- Gerenciar processos de adoção conforme as permissões do usuário.

### 2.4. Segurança

A aplicação utiliza:

- Autenticação baseada em JWT.
- Controle de autorização por perfil.
- Validação de dados recebidos pela API.
- Separação entre informações públicas e protegidas.
- Configurações específicas para desenvolvimento.

## 3. Tecnologias utilizadas

### Backend — API

| Tecnologia | Finalidade |
|---|---|
| C# | Linguagem de programação |
| .NET 10 | Plataforma de desenvolvimento |
| ASP.NET Core | Desenvolvimento da API REST |
| Entity Framework Core | Acesso e persistência de dados |
| PostgreSQL | Banco de dados relacional |
| JWT | Autenticação e autorização |
| Swagger | Documentação e testes dos endpoints |
| xUnit | Testes automatizados |

### Frontend — Aplicação Web

| Tecnologia | Finalidade |
|---|---|
| Angular | Framework da interface web |
| TypeScript | Programação do Frontend |
| HTML5 | Estrutura das páginas |
| CSS3 | Estilização e responsividade |
| Angular Router | Navegação entre páginas |
| HttpClient | Comunicação com o Backend |

### Ferramentas de desenvolvimento

- Visual Studio Code.
- Git e GitHub.
- Node.js e npm.
- .NET SDK.
- PostgreSQL.
- Navegador web.

## 4. Arquitetura do sistema

O projeto foi organizado em três partes principais.

### 4.1. Frontend — `ampara-front`

O Frontend é a parte visual do sistema, responsável por tudo que o usuário acessa e utiliza no navegador.

Suas responsabilidades incluem:

- Exibir páginas e componentes.
- Apresentar informações dos animais.
- Receber dados de formulários.
- Realizar navegação entre páginas.
- Enviar requisições HTTP para o Backend.
- Exibir respostas recebidas da API.

O Frontend foi desenvolvido utilizando Angular e TypeScript.

### 4.2. Backend — `ampara-api`

O Backend é responsável pelo funcionamento interno do sistema.

Ele recebe as requisições do Frontend, aplica as regras de negócio, realiza validações e acessa o banco de dados.

Suas principais responsabilidades incluem:

- Disponibilizar endpoints REST.
- Realizar autenticação de usuários.
- Validar dados e permissões.
- Processar solicitações de adoção.
- Consultar e persistir informações.
- Retornar respostas HTTP para o Frontend.

O Backend foi desenvolvido em C# utilizando ASP.NET Core e .NET 10.

### 4.3. Banco de dados — PostgreSQL

O PostgreSQL é utilizado para armazenar as informações persistentes da aplicação.

A comunicação entre a API e o banco é realizada utilizando Entity Framework Core.

### 4.4. Fluxo de comunicação

```text
            USUÁRIO
               |
               v
       FRONTEND (ANGULAR)
       http://localhost:4200
               |
          Requisições HTTP
               |
               v
       BACKEND (ASP.NET CORE)
       http://localhost:5154
               |
       ENTITY FRAMEWORK CORE
               |
               v
         POSTGRESQL
               |
               v
       RESPOSTA DA API
               |
               v
       FRONTEND (ANGULAR)
               |
               v
            USUÁRIO
```

O Frontend não acessa diretamente o PostgreSQL. Toda comunicação com o banco de dados ocorre por meio do Backend.

## 5. Estrutura do repositório

```text
patas-do-bem/
│
├── ampara-api/
│   ├── src/
│   │   └── Ampara/
│   │       ├── Api/
│   │       ├── Application/
│   │       ├── Domain/
│   │       ├── Infrastructure/
│   │       ├── Properties/
│   │       ├── Ampara.csproj
│   │       └── Program.cs
│   │
│   ├── tests/
│   ├── Ampara.sln
│   └── README.md
│
├── ampara-front/
│   ├── public/
│   ├── src/
│   │   └── app/
│   ├── angular.json
│   ├── package.json
│   ├── package-lock.json
│   └── proxy.conf.json
│
├── .gitignore
└── README.md
```

### Organização do Backend

| Diretório | Responsabilidade |
|---|---|
| `Api` | Endpoints e comunicação HTTP |
| `Application` | Serviços e operações da aplicação |
| `Domain` | Entidades e regras do domínio |
| `Infrastructure` | Persistência e serviços de infraestrutura |
| `tests` | Testes automatizados |
| `Program.cs` | Inicialização e configuração da aplicação |

### Organização do Frontend

| Diretório ou arquivo | Responsabilidade |
|---|---|
| `src/app` | Componentes, páginas, serviços e rotas |
| `public` | Arquivos públicos, estilos e imagens |
| `angular.json` | Configurações do Angular |
| `package.json` | Dependências e scripts |
| `proxy.conf.json` | Proxy para comunicação com a API |

## 6. Pré-requisitos

Para executar o projeto localmente, é necessário instalar:

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- [Node.js](https://nodejs.org/)
- [PostgreSQL](https://www.postgresql.org/download/)
- [Git](https://git-scm.com/downloads)

Recomenda-se utilizar o Visual Studio Code para editar e executar o projeto.

### Verificar as instalações

Abra o PowerShell e execute:

```powershell
dotnet --version
node --version
npm --version
git --version
```

Cada comando deve retornar a versão instalada.

## 7. Como obter o projeto

Clone o repositório oficial:

```powershell
git clone https://github.com/gabsjf/sistema-dac-back.git
```

Acesse a pasta criada:

```powershell
cd sistema-dac-back
```

**Importante:** todos os comandos das próximas seções consideram que o terminal está na raiz do repositório, onde estão as pastas `ampara-api` e `ampara-front`.

## 8. Configuração do banco de dados

O Backend utiliza PostgreSQL para persistência das informações.

Antes de iniciar a API, é necessário:

1. Instalar e iniciar o PostgreSQL.
2. Preparar um banco de dados chamado `ampara`.
3. Configurar os dados de conexão da API.
4. Garantir que o banco esteja acessível.

### 8.1. String de conexão

O projeto utiliza a chave `ConnectionStrings:Default` para configurar o acesso ao banco.

Exemplo:

```text
Host=localhost;Port=5432;Database=ampara;Username=postgres
```

Os valores devem corresponder à instalação local do PostgreSQL.

A porta `5432` é a porta padrão do PostgreSQL, mas pode ser diferente conforme a configuração utilizada.

### 8.2. Configuração local da senha

Por segurança, não é recomendado adicionar a senha do banco diretamente ao arquivo versionado `appsettings.json`.

Uma opção para desenvolvimento local é configurar a conexão por variável de ambiente no PowerShell:

```powershell
$env:ConnectionStrings__Default = "Host=localhost;Port=5432;Database=ampara;Username=postgres;Password=SUA_SENHA"
```

Substitua `SUA_SENHA` pela senha do PostgreSQL e ajuste a porta, caso necessário.

Essa variável permanece disponível na sessão atual do terminal.

### 8.3. Dados de demonstração

Caso utilize a inicialização de dados de demonstração configurada no Backend, defina a senha correspondente:

```powershell
$env:Seed__DemoPassword = "SUA_SENHA_DE_DEMONSTRACAO"
```

A configuração deve ser realizada antes de iniciar a API, no mesmo terminal.

## 9. Como executar o Backend

Na raiz do repositório, restaure as dependências:

```powershell
dotnet restore .\ampara-api\Ampara.sln
```

Em seguida, inicie a API:

```powershell
dotnet run --project .\ampara-api\src\Ampara --launch-profile http
```

No ambiente de desenvolvimento configurado, a API utiliza:

**http://localhost:5154**

A documentação interativa Swagger pode ser acessada em:

**http://localhost:5154/swagger**

Por meio do Swagger, é possível consultar e testar os endpoints disponíveis.

**Atenção:** mantenha esse terminal aberto enquanto estiver utilizando o Backend.

## 10. Como executar o Frontend

Abra um segundo terminal na raiz do repositório.

Acesse a pasta do Angular:

```powershell
cd .\ampara-front
```

Instale as dependências:

```powershell
npm install
```

Depois, inicie o servidor de desenvolvimento:

```powershell
npm start
```

O script de desenvolvimento utiliza a configuração de proxy do projeto.

Acesse:

**http://localhost:4200**

O Frontend deve estar disponível no navegador.

### Comunicação com o Backend

Durante o desenvolvimento, o Angular utiliza o arquivo `proxy.conf.json` para encaminhar requisições da aplicação ao Backend.

As rotas `/api` e `/uploads` são encaminhadas para:

```text
http://localhost:5154
```

Dessa forma, o Frontend consegue consumir os serviços da API sem precisar acessar diretamente outra origem pelo navegador.

**Importante:** para utilizar as funcionalidades que dependem de dados, o Backend e o PostgreSQL precisam estar funcionando.

## 11. Principais endpoints da API

A API disponibiliza rotas para autenticação, consulta de animais e operações relacionadas ao processo de adoção.

Alguns exemplos:

| Método | Endpoint | Finalidade |
|---|---|---|
| GET | `/api/public/animals` | Consultar animais públicos |
| POST | `/api/auth/register` | Cadastrar usuário |
| POST | `/api/auth/login` | Autenticar usuário |
| GET | `/api/auth/me` | Consultar usuário autenticado |

Outros endpoints estão disponíveis para operações relacionadas a animais, adoções, usuários e funcionalidades administrativas.

Para consultar as rotas e os parâmetros implementados, utilize o Swagger.

## 12. Autenticação e autorização

O sistema utiliza JWT (JSON Web Token) para autenticação.

De maneira simplificada, o processo funciona assim:

1. O usuário informa suas credenciais.
2. O Backend valida os dados.
3. A API retorna um token de autenticação quando o login é realizado com sucesso.
4. O Frontend utiliza o token nas requisições protegidas.
5. O Backend verifica o token e as permissões necessárias.

O sistema contempla perfis de acesso como:

- `Adopter` — usuário interessado em adoção.
- `Admin` — usuário administrativo.
- `SuperAdmin` — usuário com permissões administrativas superiores.

As permissões de cada operação são verificadas pelo Backend.

## 13. Testes automatizados

O Backend possui testes automatizados para verificar comportamentos da aplicação.

Para executar os testes, abra um terminal na raiz do repositório:

```powershell
dotnet test .\ampara-api
```

Na última validação realizada durante a preparação do projeto, foram registrados:

| Resultado | Quantidade |
|---|---:|
| Testes executados | 64 |
| Testes aprovados | 64 |
| Testes com falha | 0 |
| Testes ignorados | 0 |

Esses resultados correspondem à execução realizada naquela versão do projeto e não substituem a necessidade de executar novamente os testes após alterações.

## 14. Compilação do Frontend

Para verificar se o Angular compila corretamente, execute a partir da raiz do repositório:

```powershell
cd .\ampara-front
npx ng build
```

O Angular gera os arquivos de compilação no diretório `dist/`.

A compilação verifica o processo de geração da aplicação, mas não substitui os testes funcionais de todas as telas.

## 15. Problemas comuns

### O Frontend abre, mas não carrega os animais

Verifique se:

- O PostgreSQL está em execução.
- A API está funcionando.
- O Backend está acessível em `http://localhost:5154`.
- O proxy do Angular está configurado corretamente.

### Erro ao conectar ao PostgreSQL

Confira:

- Porta configurada.
- Nome do banco de dados.
- Usuário e senha.
- Serviço do PostgreSQL.
- String de conexão utilizada pelo Backend.

### Erro ao executar comandos do Angular

Verifique se o Node.js e o npm estão instalados.

Execute:

```powershell
node --version
npm --version
```

Caso as dependências ainda não tenham sido instaladas, execute `npm install` dentro da pasta `ampara-front`.

### Porta já está sendo utilizada

Verifique se já existe outra instância do Frontend ou Backend em execução.

Encerre processos antigos quando necessário e inicie novamente a aplicação.

## 16. Segurança e boas práticas

Para manter o projeto seguro e organizado:

- Não publicar senhas do banco de dados.
- Não versionar tokens JWT.
- Não adicionar credenciais reais ao README.
- Utilizar variáveis de ambiente ou configurações locais protegidas.
- Manter o `.gitignore` atualizado.
- Evitar versionar arquivos de compilação e dependências instaladas.
- Executar testes após alterações importantes.
- Revisar as modificações antes de enviá-las ao GitHub.

## 17. Versionamento e colaboração

O projeto utiliza Git para controle de versão e GitHub para armazenamento e colaboração.

O fluxo recomendado para novas alterações é:

1. Atualizar a branch principal.
2. Criar uma branch para a funcionalidade.
3. Implementar as alterações.
4. Executar os testes.
5. Registrar um commit.
6. Enviar a branch ao GitHub.
7. Criar um Pull Request.

Exemplo:

```powershell
git switch main
git pull origin main
git switch -c feature/nova-funcionalidade
```

Após implementar e testar:

```powershell
git add .
git commit -m "feat: implementa nova funcionalidade"
git push -u origin feature/nova-funcionalidade
```

As alterações podem então ser revisadas antes da integração à branch principal.

## 18. Possibilidades de evolução

O sistema pode ser ampliado futuramente com funcionalidades como:

- Melhorias no painel administrativo.
- Novos filtros de busca de animais.
- Notificações sobre o andamento das solicitações.
- Relatórios e indicadores de adoção.
- Aprimoramentos de acessibilidade.
- Ampliação da cobertura de testes.
- Implantação em ambiente de hospedagem.

Esses itens representam possibilidades de desenvolvimento futuro, não funcionalidades necessariamente implementadas.

## 19. Considerações finais

O Ampara foi desenvolvido para aplicar conhecimentos de desenvolvimento de software na construção de uma solução voltada à proteção animal.

O projeto reúne conceitos de arquitetura web, APIs REST, banco de dados relacional, autenticação, programação orientada a objetos, desenvolvimento Frontend e testes automatizados.

A separação entre Frontend e Backend facilita a manutenção do código e permite que novas funcionalidades sejam adicionadas de forma organizada.

---

**🐾 Ampara — Tecnologia a serviço da adoção responsável.**