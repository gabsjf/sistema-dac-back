# 6. API e segurança

A API é **REST**, com **JSON**, e todas as rotas começam em `/api`. Em desenvolvimento, o Swagger fica em `http://localhost:5154/swagger` (botão **Authorize** para colar o token).

## 6.1. Convenções

- Os valores enumerados trafegam como **texto**: `"Available"`, `"Pending"`, `"Admin"`…
- Datas de acolhimento são `yyyy-MM-dd`; datas e horas são ISO 8601 com fuso.
- Listagens paginadas devolvem:

```json
{ "items": [ ... ], "page": 1, "pageSize": 10, "totalCount": 7, "totalPages": 1 }
```

- Autenticação: cabeçalho `Authorization: Bearer <accessToken>`.

## 6.2. Referência dos endpoints

**Acesso:** 🌐 público · 🔑 qualquer usuário logado · 🧑 Adotante · 👥 Equipe (Admin ou SuperAdmin) · ⭐ SuperAdmin

### Autenticação — `/api/auth`

| Método e rota | Acesso | Corpo / parâmetros | Resposta |
|---|---|---|---|
| `POST /api/auth/register` | 🌐 | `fullName`, `email`, `phone?`, `password` (8+ caracteres) | `200` com `{ accessToken, expiresAt, user }` · `409` e-mail já usado · `422` senha curta |
| `POST /api/auth/login` | 🌐 | `email`, `password` | `200` com `{ accessToken, expiresAt, user }` · `401` credenciais inválidas |
| `GET /api/auth/me` | 🔑 | — | `200` com o usuário logado |

### Catálogo público — `/api/public/animals`

| Método e rota | Acesso | Parâmetros | Resposta |
|---|---|---|---|
| `GET /api/public/animals` | 🌐 | `species?` (`Canine`, `Feline`, `Other`) | Lista de animais **disponíveis** |
| `GET /api/public/animals/{id}` | 🌐 | — | `200` animal · `404` se não estiver disponível |

### Animais — `/api/animals` (👥)

| Método e rota | Corpo / parâmetros | Resposta |
|---|---|---|
| `GET /api/animals` | `search`, `species`, `status`, `page`, `pageSize` | Página de animais |
| `GET /api/animals/{id}` | — | `200` · `404` |
| `POST /api/animals` | `name`, `species`, `sex`, `size`, `approximateAgeMonths`, `intakeDate`, `description?`, `isVaccinated`, `isNeutered`, `isDewormed`, `status` | `201` com o animal · `422` regra violada |
| `PUT /api/animals/{id}` | mesmo corpo do `POST` | `200` · `404` · `422` |
| `POST /api/animals/{id}/photo` | *multipart* com o campo `file` (JPG/PNG, até 5 MB) | `200` com o animal · `422` arquivo inválido |

### Adoções — `/api/adoptions`

| Método e rota | Acesso | Corpo / parâmetros | Resposta |
|---|---|---|---|
| `POST /api/adoptions` | 🧑 | `animalId`, `phone`, `housing`, `routine`, `message?`, `acceptsTerms` | `201` · `409` já há pedido em análise · `422` animal indisponível ou termos não aceitos |
| `GET /api/adoptions/mine` | 🧑 | — | Lista das solicitações do próprio adotante |
| `GET /api/adoptions` | 👥 | `status`, `page`, `pageSize` | Página de solicitações |
| `GET /api/adoptions/{id}` | 🔑 | — | `200` (equipe, ou o adotante dono) · `404` |
| `POST /api/adoptions/{id}/approve` | 👥 | — | `200` · `422` já decidida ou animal indisponível |
| `POST /api/adoptions/{id}/reject` | 👥 | `reason` (obrigatório, até 500) | `200` · `422` |
| `POST /api/adoptions/{id}/undo-approval` | 👥 | — | `200` · `422` se não estava aprovada |

### Painel — `/api/dashboard` (👥)

`GET /api/dashboard` devolve:

```json
{
  "totalAnimals": 7, "available": 4, "inTreatment": 1, "adopted": 2,
  "pendingRequests": 1,
  "lastSixMonths": [ { "year": 2026, "month": 11, "intakes": 2, "adoptions": 1 } ]
}
```

### Equipe — `/api/users` (⭐)

| Método e rota | Corpo / parâmetros | Resposta |
|---|---|---|
| `GET /api/users` | `search`, `page`, `pageSize` | Página de usuários |
| `PUT /api/users/{id}/role` | `{ "role": "Admin" }` ou `{ "role": "Adopter" }` | `200` · `404` · `422` (não se promove a SuperAdmin, nem se altera o SuperAdmin) |

### Arquivos

`GET /uploads/animals/{arquivo}` serve as fotos. Não exige login, pois as fotos aparecem no catálogo público.

## 6.3. Códigos de erro (ProblemDetails)

Todos os erros seguem o padrão **RFC 7807**. O campo `detail` pode ser mostrado ao usuário.

```json
{ "status": 409, "title": "Conflito", "detail": "Você já tem uma solicitação em análise para este animal." }
```

| Código | Quando | Origem |
|---|---|---|
| `400` | Dados no formato errado ou campo obrigatório ausente | Validação automática dos DTOs |
| `401` | Sem login, token inválido ou credenciais erradas | `UnauthorizedException` / JWT |
| `403` | Logado, mas o papel não permite a ação | `[Authorize(Roles = …)]` |
| `404` | Recurso inexistente (ou que o usuário não pode ver) | `NotFoundException` |
| `409` | Conflito, como e-mail repetido ou pedido duplicado | `ConflictException` |
| `422` | Regra de negócio violada | `DomainException` |
| `500` | Erro inesperado; a mensagem é genérica e o detalhe vai só para o log | Qualquer outra exceção |

## 6.4. Autenticação e autorização

### Como funciona

1. O usuário envia e-mail e senha para `POST /api/auth/login`.
2. A API confere a senha com o *hash* guardado. Se estiver correta, emite um **JWT** assinado.
3. O token carrega: `sub` (id), `email`, `name` e `role`. Vale **8 horas**.
4. O front envia o token em todas as chamadas à API (via *interceptor*).
5. A API valida assinatura, emissor, público e validade; e confere o papel em cada rota.

### Matriz de permissões

| Ação | Visitante | Adotante | Admin | SuperAdmin |
|---|:---:|:---:|:---:|:---:|
| Ver catálogo público | ✅ | ✅ | ✅ | ✅ |
| Criar conta e entrar | ✅ | ✅ | ✅ | ✅ |
| Pedir adoção e ver os próprios pedidos | ❌ | ✅ | ❌ | ❌ |
| Ver qualquer solicitação | ❌ | só as próprias | ✅ | ✅ |
| Aprovar, recusar e desfazer adoção | ❌ | ❌ | ✅ | ✅ |
| Cadastrar e editar animais | ❌ | ❌ | ✅ | ✅ |
| Ver o painel | ❌ | ❌ | ✅ | ✅ |
| Promover e rebaixar pessoas | ❌ | ❌ | ❌ | ✅ |

> A equipe **não** pede adoção: a rota `POST /api/adoptions` é restrita ao papel Adotante.

## 6.5. Análise de segurança

| Ameaça | Como o sistema trata | Situação |
|---|---|---|
| **Acesso indevido** (usar uma rota sem permissão) | Autorização por papel em cada rota, **no servidor**; o adotante só lê os próprios pedidos | ✅ |
| **Elevação de privilégio** | O cadastro público cria sempre Adotante; ninguém vira SuperAdmin; o corpo do cadastro não aceita o campo `role` | ✅ |
| **Senhas expostas** | Guardadas só como hash PBKDF2 com sal; nunca saem da API | ✅ |
| **SQL injection** | Todas as consultas passam pelo EF Core, que usa parâmetros | ✅ |
| **XSS** | O Angular escapa o conteúdo por padrão; não há `innerHTML` com dados do usuário | ✅ |
| **Upload malicioso** | Limite de 5 MB; o tipo é conferido pelos primeiros bytes do arquivo; o nome é gerado pelo servidor; a remoção nunca sai da pasta de uploads | ✅ |
| **Vazamento em erros** | Erros inesperados viram um `500` genérico | ✅ |
| **Segredos no Git** | Chave JWT e senhas ficam no `user-secrets`; a API não inicia com chave fraca (menos de 32 caracteres) | ✅ |
| **CORS** | Apenas as origens da configuração `Cors:AllowedOrigins` (`http://localhost:4200`) | ✅ |
| **Transporte** | Fora de desenvolvimento a API redireciona HTTP para HTTPS. Em desenvolvimento usa HTTP | 🟡 |
| **Roubo do token** | O token fica no `sessionStorage`: some ao fechar a aba, mas é legível por JavaScript se houver XSS. A alternativa mais segura seria cookie `httpOnly` | 🟡 |
| **Revogação e renovação do token** | Não existe renovação (*refresh token*) nem revogação antes dos 8 horas | 🔵 |
| **Força bruta no login** | Não há limite de tentativas nem bloqueio temporário | 🔵 |
| **Privacidade (LGPD)** | Há consentimento no pedido de adoção; falta política de privacidade e opção de exclusão de dados | 🟡 |
