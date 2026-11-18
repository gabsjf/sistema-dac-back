# Patas do Bem · Gestão com Ampara

## Estrutura do Projeto

- `patas-backend/` — Aplicação Java 17 + Spring Boot (`controller`, `service`, `model`, `config`, `pom.xml`)
- `patas-frontend/` — Páginas HTML Thymeleaf (`templates/`) e arquivos estáticos (`static/css`, `static/js`, `static/assets`)
- `patas-infra/` — Scripts de execução (`iniciar.ps1`) e documentação

## Como iniciar

```powershell
.\patas-infra\iniciar.ps1
```

Ou diretamente com Maven:

```powershell
mvn -f patas-backend/pom.xml spring-boot:run
```

Acesse: `http://localhost:8080/` (ou `/mapa` para navegar por todas as telas).
