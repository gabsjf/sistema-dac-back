# Ampara — Documentação do projeto

Documentação técnica e funcional do **Ampara**, sistema de gestão e adoção animal da ONG **Patas do Bem**.
Complementa o [README da raiz](../README.md) e o [README da API](../ampara-api/README.md), que explicam como **executar** o projeto. Aqui está o **o quê** e o **porquê**.

| Campo | Valor |
|---|---|
| Versão da documentação | 1.0 |
| Data | 24/11/2026 |
| Base de análise | Código do repositório `sistema-dac-back` (branch `main`) + alterações locais ainda não enviadas ao GitHub (veja a [seção 8](08-demo-limitacoes-e-roadmap.md#2-alinhamento-entre-o-código-e-o-quadro-do-trello)) |
| Gestão das tarefas | Trello — *Ampara · Trabalho final* |

## Como ler

| # | Documento | Responde a |
|---|---|---|
| 1 | [Visão e escopo](01-visao-e-escopo.md) | Qual problema resolvemos, para quem, e o que está dentro e fora do escopo |
| 2 | [Requisitos](02-requisitos.md) | O que o sistema deve fazer (RF), como deve se comportar (RNF) e quais regras de negócio (RN) |
| 3 | [Casos de uso e histórias](03-casos-de-uso.md) | Quem faz o quê no sistema |
| 4 | [Modelagem](04-modelagem.md) | Banco de dados, classes de domínio, estados e sequências |
| 5 | [Arquitetura e decisões](05-arquitetura.md) | Como o sistema é dividido e por que foi feito assim |
| 6 | [API e segurança](06-api-e-seguranca.md) | Endpoints, autenticação, autorização e tratamento de erros |
| 7 | [Testes](07-testes.md) | O que é testado, como, e o que ainda falta |
| 8 | [Demo, limitações e roadmap](08-demo-limitacoes-e-roadmap.md) | Roteiro da apresentação, estado real do projeto e próximos passos |

## Legenda de situação

Todo requisito e funcionalidade citados nesta documentação recebem uma situação, verificada no código:

| Situação | Significado |
|---|---|
| ✅ **Implementado** | Existe no código e foi verificado |
| 🟡 **Parcial** | Existe parte da funcionalidade; a lacuna está descrita |
| 🔵 **Planejado** | Está no Trello, mas ainda não foi iniciado |
| ⚪ **Fora do escopo** | Decidido não fazer nesta fase |

## Diagramas

Os diagramas estão em [Mermaid](https://mermaid.js.org/), dentro dos próprios arquivos `.md`. O GitHub e o VS Code (com a extensão *Markdown Preview Mermaid Support*) os exibem sem nenhuma ferramenta extra, e o texto fica versionado junto com o código.

## Gerar o PDF

O arquivo `Ampara-Documentacao.pdf` reúne todos os documentos, com capa e diagramas. Para gerá-lo de novo depois de editar os `.md`:

```bash
# na pasta docs
node _build/gerar-pdf.mjs
```

Precisa de Node.js, Google Chrome e internet (as bibliotecas `marked` e `mermaid` vêm de uma CDN). O script confere se todos os diagramas foram desenhados antes de criar o PDF. Se o Chrome estiver em outro caminho, defina a variável `CHROME_PATH`.

## Equipe

| Pessoa | Papel no Trello |
|---|---|
| Gabriel | Gerenciamento |
| Heitor | Back-end |
| Lucas | Banco de dados |
| Pedro | Front-end |
| Guilherme | Protótipo e front-end |
| Glauber | Testes automatizados |
