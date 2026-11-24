# 1. Visão e escopo

## 1.1. Contexto

A **Patas do Bem** é uma ONG de proteção animal. A equipe acolhe animais, cuida deles e procura famílias para adotá-los. Sem um sistema, o controle costuma ficar espalhado em planilhas e mensagens: quem está disponível, quem está em tratamento, quem pediu para adotar e qual foi a decisão.

O **Ampara** centraliza esse trabalho em um sistema web.

## 1.2. Objetivo

> Facilitar e organizar a **adoção responsável**, dando à ONG um lugar único para cadastrar animais e decidir sobre as solicitações, e dando ao público uma forma simples de conhecer os animais e pedir a adoção.

Objetivos específicos:

1. Divulgar os animais disponíveis para adoção em um catálogo público.
2. Permitir que uma pessoa se cadastre e peça a adoção de um animal pela internet.
3. Permitir que a equipe aprove ou recuse cada pedido, registrando o motivo da recusa.
4. Manter a situação de cada animal sempre coerente com as decisões tomadas.
5. Dar à ONG indicadores simples para acompanhar o trabalho.
6. Proteger os dados pessoais e restringir cada ação a quem tem permissão.

## 1.3. Atores

| Ator | Descrição | Como entra no sistema |
|---|---|---|
| **Visitante** | Qualquer pessoa na internet, sem conta | Acessa a área pública |
| **Adotante** (`Adopter`) | Pessoa cadastrada que quer adotar | Cria a própria conta |
| **Admin** (`Admin`) | Integrante da equipe da ONG | É promovido pelo SuperAdmin |
| **SuperAdmin** (`SuperAdmin`) | Responsável pela ONG; existe um só | É criado pelo *seed* do sistema |

## 1.4. Escopo

### Dentro do escopo (MVP)

| Área | Funcionalidades |
|---|---|
| Acesso | Cadastro, login e controle de acesso por papel |
| Catálogo | Lista e ficha pública dos animais disponíveis |
| Animais | Cadastro, edição, foto e controle de situação |
| Adoções | Pedido, acompanhamento, aprovação, recusa e desfazer aprovação |
| Gestão | Painel com indicadores e gráfico mensal |
| Equipe | Promover pessoas a Admin e voltar a Adotante |

### Planejado (quadro do Trello, ainda não iniciado)

| Área | Funcionalidade | Prioridade no Trello |
|---|---|---|
| Tempo real | Notificações por SignalR e indicador de conexão | Próxima feature grande |
| Adoções | Estados *recebida/em análise/concluída*, termo de adoção em PDF e registro da entrega | MVP |
| Animais | Módulo de saúde e cuidados (atendimentos e próximos cuidados) | MVP |
| Acesso | Recuperar senha e verificar e-mail | Desejável |
| Gestão | Voluntários, financeiro e relatórios com exportação CSV | Desejável |
| Portal | Voluntariado, doação via PIX e páginas institucionais | Desejável |
| Plataforma | Isolamento de dados por ONG e tela de configurações | MVP |
| Infra | Integração contínua (GitHub Actions) e deploy | Adiado / MVP |

### Fora do escopo desta fase

- Aplicativo móvel nativo.
- Pagamentos reais (a doação via PIX seria apenas informativa).
- Integração com sistemas externos de terceiros.

## 1.5. Premissas e restrições

- Trabalho acadêmico, desenvolvido por seis pessoas, com tecnologias definidas: **.NET 10**, **PostgreSQL** e **Angular**.
- O sistema atende **uma única ONG** (a *Patas do Bem*). O isolamento entre várias ONGs consta no Trello, mas não foi implementado.
- A execução é local (`localhost`). O deploy em ambiente público consta no Trello como pendente.
- As fotos dos animais ficam no disco do servidor da API.

## 1.6. Glossário

| Termo | Significado |
|---|---|
| Acolhimento | Entrada de um animal na ONG. Tem uma data (`IntakeDate`). |
| Situação | Estado do animal: *Disponível*, *Em tratamento* ou *Adotado*. |
| Solicitação | Pedido de adoção de um animal feito por um adotante. |
| Equipe | Admins e o SuperAdmin. |
| Seed | Dados iniciais criados automaticamente em desenvolvimento. |
| JWT | *JSON Web Token*, o "crachá" digital enviado em cada requisição. |
