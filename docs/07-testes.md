# 7. Testes

## 7.1. Estratégia

| Nível | O que valida | Ferramenta | Situação |
|---|---|---|---|
| **Unidade — domínio** | Regras de negócio das entidades | xUnit | ✅ |
| **Unidade — aplicação** | Casos de uso, com repositórios simulados (sem banco) | xUnit | ✅ |
| **Unidade — infraestrutura** | JWT e hash de senha | xUnit | ✅ |
| **Unidade — API** | Conversão de exceções em respostas HTTP | xUnit | ✅ |
| **Integração da API** | Rotas, autorização e banco de dados de verdade | — | 🔵 Não existe |
| **Front-end** | Componentes, guards e interceptor | Vitest (configurado) | 🔵 Só o teste padrão do projeto |
| **Ponta a ponta (E2E)** | Fluxo completo no navegador | — | 🔵 Não existe |
| **Aceitação manual** | Roteiros por perfil (seção 7.4) | Navegador | ✅ Roteiro definido |

## 7.2. Como executar

```bash
# na pasta ampara-api
dotnet test
```

**Resultado da última execução (24/11/2026):** `Aprovado! — Com falha: 0, Aprovado: 64, Ignorado: 0, Total: 64`, em cerca de 3 segundos. Os testes **não precisam de banco de dados**.

## 7.3. Cobertura por regra

Os 64 testes ficam em `ampara-api/tests/Ampara.UnitTests`.

### Domínio

| Arquivo | Testes | Regras cobertas |
|---|---:|---|
| `Domain/AnimalTests.cs` | 10 | RN-05, RN-06, RN-07 (não nascer como adotado, não virar adotado nem sair de adotado por edição) e a regra de que só animal *Disponível* pode ser adotado |
| `Domain/AdoptionRequestTests.cs` | 9 | RN-09, RN-10, RN-12 (decidir só uma vez), RN-13, RN-14 (animal já adotado por outra família), RN-15 |
| `Domain/UserTests.cs` | 6 | RN-02 (SuperAdmin intocável e inalcançável), e-mail em minúsculas e válido, nome obrigatório |

### Aplicação (casos de uso)

| Arquivo | Testes | Regras cobertas |
|---|---:|---|
| `Application/AuthServiceTests.cs` | 7 | RN-01, RN-03 (senha curta, e-mail repetido), login válido e inválido, `me` |
| `Application/AnimalServiceTests.cs` | 8 | Cadastro e edição, paginação limitada (RN-18), upload de foto PNG com troca da anterior, arquivo que não é imagem, arquivo maior que 5 MB (RN-08), animal adotado fora do catálogo |
| `Application/AdoptionServiceTests.cs` | 9 | Criar pedido, RN-11 (pedido duplicado), animal inexistente, RN-17 (outro adotante não vê), equipe vê qualquer um, aprovar e recusar |
| `Application/DashboardServiceTests.cs` | 2 | Contagens por situação e agrupamento dos últimos 6 meses |
| `Application/UserServiceTests.cs` | 4 | Promover e rebaixar, usuário inexistente, bloqueio de SuperAdmin, busca e paginação |

### Infraestrutura e API

| Arquivo | Testes | Regras cobertas |
|---|---:|---|
| `Infrastructure/SecurityTests.cs` | 2 | Token com as informações do usuário e validade correta; hash só confere a senha original |
| `Api/GlobalExceptionHandlerTests.cs` | 2 | Exceções conhecidas viram o status certo; erro inesperado vira `500` sem vazar detalhes |

**Total:** 10 + 9 + 6 + 7 + 8 + 9 + 2 + 4 + 2 + 2 = **59 métodos de teste**, que geram **64 execuções** (alguns usam vários conjuntos de dados).

## 7.4. Lacunas conhecidas

| Lacuna | Risco | Sugestão |
|---|---|---|
| **Desfazer aprovação** (`UndoApproval`, RN-16) não tem teste automatizado, embora seja usado pela tela de Adoções | Médio | Adicionar testes de domínio e de serviço |
| Nenhum teste de **autorização por rota** (um adotante chamando rota da equipe) | Médio | Testes de integração com `WebApplicationFactory` |
| Nenhum teste contra **PostgreSQL real** (consultas, filtros, migration) | Médio | Integração com um banco em contêiner |
| **Front-end** sem testes de guards, interceptor e telas | Médio | Testes com Vitest nos guards e no interceptor |
| Testes do SignalR e do isolamento por ONG (previstos no Trello) | Baixo | Dependem de funcionalidades ainda não implementadas |

## 7.5. Roteiro de testes manuais (aceitação)

Pré-condição: banco, API e front no ar (veja o README) e os dados de demonstração criados.
Contas: `admin@ampara.local` (SuperAdmin) e `adotante@ampara.local` (Adotante).

| ID | Perfil | Passos | Resultado esperado |
|---|---|---|---|
| CT-01 | Visitante | Abrir a página inicial | Lista só os animais *Disponíveis* |
| CT-02 | Visitante | Abrir a ficha de um animal | Mostra dados e foto |
| CT-03 | Visitante | Cadastrar-se com senha de 7 caracteres | Erro: senha deve ter pelo menos 8 caracteres |
| CT-04 | Visitante | Cadastrar-se com e-mail já existente | Erro de conflito (e-mail já usado) |
| CT-05 | Visitante | Cadastrar-se corretamente | Conta criada como Adotante e já logada |
| CT-06 | Qualquer | Entrar com senha errada | Mensagem "E-mail ou senha incorretos" |
| CT-07 | Adotante | Pedir adoção sem aceitar os termos | Erro de validação |
| CT-08 | Adotante | Pedir adoção de um animal disponível | Pedido criado; aparece em "Minhas solicitações" como *Em análise* |
| CT-09 | Adotante | Pedir de novo o mesmo animal | Erro: já há solicitação em análise |
| CT-10 | Adotante | Abrir `/admin/visao-geral` pelo endereço | Redirecionado para a página inicial |
| CT-11 | Admin | Entrar | Vai para a Visão geral; vê os indicadores e o gráfico |
| CT-12 | Admin | Cadastrar um animal com foto | Aparece na lista; indicadores atualizados |
| CT-13 | Admin | Cadastrar animal com nome vazio | Mensagem "Informe o nome do animal" |
| CT-14 | Admin | Editar um animal *Adotado* | O campo situação fica bloqueado |
| CT-15 | Admin | Aprovar uma solicitação | Solicitação *Aprovada*; animal vira *Adotado*; some do catálogo |
| CT-16 | Admin | Aprovar outro pedido do mesmo animal | Erro: animal não está disponível |
| CT-17 | Admin | Recusar sem motivo | Erro: motivo obrigatório |
| CT-18 | Admin | Desfazer uma aprovação | Pedido volta a *Em análise*; animal volta a *Disponível* |
| CT-19 | Admin | Abrir `/admin/equipe` | Redirecionado para a Visão geral (só SuperAdmin) |
| CT-20 | SuperAdmin | Promover um adotante a Admin | Papel alterado; a pessoa passa a acessar o painel |
| CT-21 | SuperAdmin | Tentar alterar o próprio papel | Não é permitido (o SuperAdmin não tem seletor) |
| CT-22 | Admin | Abrir "Financeiro" no menu | Página "Em desenvolvimento" |
