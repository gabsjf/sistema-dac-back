# Ampara — Sistema de Gestão e Adoção Animal

Sistema web para gerenciamento de animais e solicitações de adoção.

## Tecnologias
- Backend: C# e .NET 10
- Frontend: Angular e TypeScript
- Banco de dados: PostgreSQL
- Autenticação: JWT

## Estrutura
- ampara-api/: API e testes automatizados.
- ampara-front/: aplicação web Angular.

## Requisitos
- .NET 10 SDK
- Node.js e npm
- PostgreSQL

## Executar o Backend

```powershell
dotnet restore .\ampara-api\Ampara.sln
dotnet run --project .\ampara-api\src\Ampara\Ampara.csproj
```

## Executar o Frontend

```powershell
cd .\ampara-front
npm install
npx ng serve --proxy-config proxy.conf.json
```

Acesse http://localhost:4200.

## Testes

```powershell
dotnet test .\ampara-api
```

## Segurança
Não publique senhas, tokens ou credenciais pessoais.
