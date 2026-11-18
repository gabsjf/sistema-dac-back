package br.org.patasdobem.model;

public class SolicitacaoAdocao {
    private String codigo;
    private String animalId;
    private String animalNome;
    private String adotanteNome;
    private String email;
    private String telefone;
    private String moradia;
    private String rotina;
    private String experiencia;
    private String status; // EM_ANALISE, APROVADA, TERMO_ACEITO
    private String statusLabel;

    public SolicitacaoAdocao() {
    }

    public SolicitacaoAdocao(String codigo, String animalId, String animalNome, String adotanteNome,
                             String email, String telefone, String moradia, String rotina,
                             String experiencia, String status, String statusLabel) {
        this.codigo = codigo;
        this.animalId = animalId;
        this.animalNome = animalNome;
        this.adotanteNome = adotanteNome;
        this.email = email;
        this.telefone = telefone;
        this.moradia = moradia;
        this.rotina = rotina;
        this.experiencia = experiencia;
        this.status = status;
        this.statusLabel = statusLabel;
    }

    public String getCodigo() {
        return codigo;
    }

    public void setCodigo(String codigo) {
        this.codigo = codigo;
    }

    public String getAnimalId() {
        return animalId;
    }

    public void setAnimalId(String animalId) {
        this.animalId = animalId;
    }

    public String getAnimalNome() {
        return animalNome;
    }

    public void setAnimalNome(String animalNome) {
        this.animalNome = animalNome;
    }

    public String getAdotanteNome() {
        return adotanteNome;
    }

    public void setAdotanteNome(String adotanteNome) {
        this.adotanteNome = adotanteNome;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getTelefone() {
        return telefone;
    }

    public void setTelefone(String telefone) {
        this.telefone = telefone;
    }

    public String getMoradia() {
        return moradia;
    }

    public void setMoradia(String moradia) {
        this.moradia = moradia;
    }

    public String getRotina() {
        return rotina;
    }

    public void setRotina(String rotina) {
        this.rotina = rotina;
    }

    public String getExperiencia() {
        return experiencia;
    }

    public void setExperiencia(String experiencia) {
        this.experiencia = experiencia;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getStatusLabel() {
        return statusLabel;
    }

    public void setStatusLabel(String statusLabel) {
        this.statusLabel = statusLabel;
    }
}
