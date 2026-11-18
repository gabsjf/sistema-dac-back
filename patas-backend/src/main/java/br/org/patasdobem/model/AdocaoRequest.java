package br.org.patasdobem.model;

public class AdocaoRequest {
    private String animalId;
    private String animalNome;
    private String nomeInteressado;
    private String email;
    private String telefone;
    private String mensagem;

    public AdocaoRequest() {
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

    public String getNomeInteressado() {
        return nomeInteressado;
    }

    public void setNomeInteressado(String nomeInteressado) {
        this.nomeInteressado = nomeInteressado;
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

    public String getMensagem() {
        return mensagem;
    }

    public void setMensagem(String mensagem) {
        this.mensagem = mensagem;
    }
}
