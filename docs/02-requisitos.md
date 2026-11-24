# 2. Requisitos

Os requisitos têm identificadores estáveis (**RF**, **RNF**, **RN**) para que testes, casos de uso e cartões do Trello possam apontar para eles. A coluna **Situação** segue a [legenda](README.md#legenda-de-situação) e foi verificada no código.

> **Importante:** o quadro do Trello foi alinhado ao código em 24/11/2026. O que não existe no código aparece aqui como 🔵 Planejado ou 🟡 Parcial. O que foi ajustado no quadro está em [8. Demo, limitações e roadmap](08-demo-limitacoes-e-roadmap.md#2-alinhamento-entre-o-código-e-o-quadro-do-trello).

## 2.1. Requisitos funcionais (RF)

### Acesso e perfis

| ID | Requisito | Ator | Situação | Cartão no Trello |
|---|---|---|---|---|
| RF-01 | O visitante deve poder criar uma conta informando nome, e-mail, telefone (opcional) e senha. A conta nasce sempre como Adotante. | Visitante | ✅ Implementado | Login com JWT e papéis |
| RF-02 | O usuário deve poder entrar com e-mail e senha e receber um token de acesso. | Todos | ✅ Implementado | Login com JWT e papéis |
| RF-03 | O sistema deve restringir cada funcionalidade ao papel autorizado (Adotante, Admin, SuperAdmin). | Todos | ✅ Implementado | Login com JWT e papéis |
| RF-04 | O SuperAdmin deve poder listar pessoas, buscar por nome ou e-mail, e promover alguém a Admin ou devolvê-lo a Adotante. | SuperAdmin | ✅ Implementado (API e tela *Equipe*) | *(não consta no quadro)* |
| RF-05 | O usuário deve poder recuperar a senha e verificar o e-mail. | Todos | 🔵 Planejado | Recuperar senha e verificar e-mail |
| RF-06 | Os dados de cada ONG devem ficar isolados, com tela de configurações da organização. | Admin | 🔵 Planejado (hoje o sistema atende uma só ONG; a tela *Configurações* é um aviso "Em desenvolvimento") | Isolar dados por ONG e tela Configurações |

### Animais

| ID | Requisito | Ator | Situação | Cartão no Trello |
|---|---|---|---|---|
| RF-07 | A equipe deve poder cadastrar e editar um animal (nome, espécie, sexo, porte, idade aproximada, data de acolhimento, descrição e se é vacinado, castrado e vermifugado). | Equipe | ✅ Implementado (formulário único em janela, e não em 3 passos) | Cadastro e edição de animal |
| RF-08 | A equipe deve poder enviar uma foto do animal (JPG ou PNG, até 5 MB). | Equipe | ✅ Implementado | Cadastro e edição de animal |
| RF-09 | A equipe deve poder listar os animais com busca por nome, filtro por espécie e situação, e paginação. | Equipe | ✅ Implementado | Lista de animais |
| RF-10 | O sistema deve controlar a situação do animal (*Disponível*, *Em tratamento*, *Adotado*) com transições válidas. | Sistema | ✅ Implementado | Cadastro e edição de animal |
| RF-11 | A ficha do animal deve mostrar dados, foto, situação e o histórico de saúde e de adoção. | Todos | 🟡 Parcial (existe a ficha pública com dados e foto; não há histórico) | Ficha do animal |
| RF-12 | A equipe deve poder registrar atendimentos (vacina, consulta, medicação) e ver os próximos cuidados. | Equipe | 🔵 Planejado (o sistema guarda apenas três indicadores: vacinado, castrado, vermifugado) | Saúde e cuidados |

### Adoções

| ID | Requisito | Ator | Situação | Cartão no Trello |
|---|---|---|---|---|
| RF-13 | O público deve poder ver a lista e a ficha dos animais disponíveis, sem login. | Visitante | ✅ Implementado | Home e animais para adoção |
| RF-14 | O adotante deve poder solicitar a adoção de um animal disponível, informando telefone, moradia, rotina, mensagem e aceitando os termos. | Adotante | ✅ Implementado | Solicitar adoção |
| RF-15 | O adotante deve poder acompanhar o andamento das próprias solicitações. | Adotante | ✅ Implementado | *(não consta no quadro)* |
| RF-16 | A equipe deve poder listar solicitações (com filtro por situação), ver os detalhes, aprovar, recusar com motivo e desfazer uma aprovação. | Equipe | ✅ Implementado | Lista de adoções e análise de candidatura |
| RF-17 | O sistema deve ter os estados *recebida*, *em análise* e *concluída*, gerar o termo de adoção em PDF e registrar a entrega do animal. | Equipe | 🔵 Planejado (hoje os estados são *Pendente*, *Aprovada* e *Recusada*) | Fluxo de adoção no back · Termo, entrega e conclusão |

### Gestão e plataforma

| ID | Requisito | Ator | Situação | Cartão no Trello |
|---|---|---|---|---|
| RF-18 | A equipe deve ver um painel com total de animais, disponíveis, em tratamento, adotados, solicitações pendentes e o gráfico de acolhimentos × adoções dos últimos 6 meses. | Equipe | ✅ Implementado (tela *Visão geral*) | Painel de gestão |
| RF-19 | A equipe deve poder cadastrar voluntários e aprovar inscrições feitas no portal. | Equipe | 🔵 Planejado | Voluntários |
| RF-20 | A equipe deve poder registrar entradas e saídas financeiras e ver o saldo. | Equipe | 🔵 Planejado | Financeiro |
| RF-21 | A equipe deve poder gerar relatórios por período e exportar em CSV. | Equipe | 🔵 Planejado | Relatórios |
| RF-22 | A equipe deve receber notificações em tempo real (nova solicitação, mudança de status), por SignalR, com indicador de conexão. | Equipe | 🔵 Planejado | Tempo real (SignalR) — 5 cartões |
| RF-23 | O portal deve ter voluntariado, doação via PIX e páginas institucionais. | Visitante | 🔵 Planejado | Voluntariado, doação e institucionais |

**Resumo:** dos 23 requisitos, 13 estão ✅ implementados, 1 está 🟡 parcial (RF-11) e 9 estão 🔵 planejados (RF-05, 06, 12, 17, 19, 20, 21, 22 e 23).

## 2.2. Requisitos não funcionais (RNF)

| ID | Categoria | Requisito | Situação |
|---|---|---|---|
| RNF-01 | Segurança | Senhas devem ser guardadas apenas como *hash* com sal (PBKDF2); nunca em texto. | ✅ |
| RNF-02 | Segurança | A autenticação deve usar JWT com expiração (8 horas). | ✅ |
| RNF-03 | Segurança | A autorização deve ser verificada **na API**, e não só na interface. | ✅ |
| RNF-04 | Segurança | Erros inesperados não devem expor detalhes internos ao usuário. | ✅ |
| RNF-05 | Segurança | Segredos (chave JWT, senha do banco) não devem ir para o repositório. | ✅ (`user-secrets`) |
| RNF-06 | Privacidade | O pedido de adoção deve registrar o consentimento do solicitante (maioridade e análise pela ONG). | 🟡 (há o aceite obrigatório; falta política de privacidade completa) |
| RNF-07 | Manutenibilidade | O back-end deve ser organizado em camadas, com regras de negócio isoladas no domínio. | ✅ |
| RNF-08 | Testabilidade | As regras de negócio devem ser testáveis sem banco de dados. | ✅ (64 testes unitários) |
| RNF-09 | Interoperabilidade | A API deve seguir REST com JSON, descrever os erros em *ProblemDetails* (RFC 7807) e ter documentação Swagger. | ✅ |
| RNF-10 | Desempenho | Listagens devem ser paginadas, com tamanho máximo de 50 itens por página. | ✅ |
| RNF-11 | Integridade | Operações que mudam mais de uma entidade (ex.: aprovar adoção e marcar o animal) devem ser gravadas juntas. | ✅ (uma única gravação por operação) |
| RNF-12 | Portabilidade | O sistema deve rodar em Windows, Linux e macOS com .NET 10, Node.js e PostgreSQL 16 (ou superior). | ✅ |
| RNF-13 | Usabilidade | A interface deve ser em português do Brasil e as mensagens de erro da API devem poder ser mostradas ao usuário. | ✅ |
| RNF-14 | Disponibilidade | O sistema deve ter ambiente público de demonstração. | 🔵 (deploy pendente no Trello) |

## 2.3. Regras de negócio (RN)

### Usuários e papéis

| ID | Regra | Onde é garantida |
|---|---|---|
| RN-01 | Todo cadastro público cria um **Adotante**. | `AuthService.RegisterAsync` |
| RN-02 | Existe um único **SuperAdmin**, criado pelo seed. Ninguém é promovido a SuperAdmin e o papel dele não muda. | `User.ChangeRole` |
| RN-03 | A senha tem no mínimo 8 caracteres. O e-mail é único e gravado em minúsculas. | `AuthService`, `User.Register` |
| RN-04 | Só o SuperAdmin promove alguém a Admin ou o devolve a Adotante. | `UsersController` (`[Authorize(Roles = SuperAdmin)]`) |

### Animais

| ID | Regra | Onde é garantida |
|---|---|---|
| RN-05 | Nome com até 80 caracteres e obrigatório; descrição com até 1000; idade entre 0 e 360 meses. | `Animal.Apply` |
| RN-06 | A data de acolhimento não pode estar no futuro. | `Animal.Apply` |
| RN-07 | Um animal **não nasce** como *Adotado*. A situação *Adotado* só é atingida pela aprovação de uma adoção e só deixa de existir ao desfazer essa aprovação. | `Animal.Register`, `Animal.Update` |
| RN-08 | A foto deve ser JPG ou PNG, com até 5 MB. O tipo é conferido pelo conteúdo do arquivo, e não só pela extensão. Ao trocar a foto, a anterior é apagada. | `AnimalService.UploadPhotoAsync` |
| RN-09 | Só animais **Disponíveis** aparecem no catálogo público e podem receber solicitações. | `AnimalService`, `AdoptionRequest.Submit` |

### Adoções

| ID | Regra | Onde é garantida |
|---|---|---|
| RN-10 | Para solicitar, é preciso informar telefone, moradia e rotina, e aceitar os termos. | `AdoptionRequest.Submit` |
| RN-11 | Um adotante não pode ter duas solicitações em análise para o mesmo animal. (resposta 409) | `AdoptionService.SubmitAsync` |
| RN-12 | Só a equipe decide. Uma solicitação só pode ser decidida **uma vez** (estava *Pendente*). | `AdoptionRequest.EnsurePending` |
| RN-13 | **Aprovar** marca a solicitação como *Aprovada* e o animal como *Adotado*. | `AdoptionRequest.Approve` |
| RN-14 | Se dois adotantes pedem o mesmo animal, a ONG aprova um; aprovar o outro é bloqueado, pois o animal já não está disponível. | `Animal.MarkAsAdopted` |
| RN-15 | **Recusar** exige um motivo (até 500 caracteres) e mantém o animal disponível. | `AdoptionRequest.Reject` |
| RN-16 | **Desfazer a aprovação** devolve a solicitação a *Pendente* e o animal a *Disponível*. | `AdoptionRequest.UndoApproval` |
| RN-17 | O adotante só enxerga as **próprias** solicitações. Pedir a de outra pessoa responde "não encontrada". | `AdoptionService.GetAsync` |

### Listagens

| ID | Regra | Onde é garantida |
|---|---|---|
| RN-18 | Páginas começam em 1, têm 10 itens por padrão e no máximo 50. | `Paging.Normalize` |

## 2.4. Rastreabilidade (RN → testes)

As regras do domínio têm testes automatizados. O detalhe está em [7. Testes](07-testes.md#cobertura-por-regra).
