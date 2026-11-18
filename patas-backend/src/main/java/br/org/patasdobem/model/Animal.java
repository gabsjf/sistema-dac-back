package br.org.patasdobem.model;

import java.util.List;

public class Animal {
    private String id;
    private String nome;
    private String especie;
    private String sexo;
    private String idade;
    private String porte;
    private String status;
    private String statusDetalhado;
    private String descricao;
    private String localizacao;
    private String fotoCard;
    private String fotoPerfil;
    private List<String> cuidados;

    public Animal() {
    }

    public Animal(String id, String nome, String especie, String sexo, String idade, String porte,
                  String status, String statusDetalhado, String descricao, String localizacao,
                  String fotoCard, String fotoPerfil, List<String> cuidados) {
        this.id = id;
        this.nome = nome;
        this.especie = especie;
        this.sexo = sexo;
        this.idade = idade;
        this.porte = porte;
        this.status = status;
        this.statusDetalhado = statusDetalhado;
        this.descricao = descricao;
        this.localizacao = localizacao;
        this.fotoCard = fotoCard;
        this.fotoPerfil = fotoPerfil;
        this.cuidados = cuidados;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getNome() {
        return nome;
    }

    public void setNome(String nome) {
        this.nome = nome;
    }

    public String getEspecie() {
        return especie;
    }

    public void setEspecie(String especie) {
        this.especie = especie;
    }

    public String getSexo() {
        return sexo;
    }

    public void setSexo(String sexo) {
        this.sexo = sexo;
    }

    public String getIdade() {
        return idade;
    }

    public void setIdade(String idade) {
        this.idade = idade;
    }

    public String getPorte() {
        return porte;
    }

    public void setPorte(String porte) {
        this.porte = porte;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public String getStatusDetalhado() {
        return statusDetalhado;
    }

    public void setStatusDetalhado(String statusDetalhado) {
        this.statusDetalhado = statusDetalhado;
    }

    public String getDescricao() {
        return descricao;
    }

    public void setDescricao(String descricao) {
        this.descricao = descricao;
    }

    public String getLocalizacao() {
        return localizacao;
    }

    public void setLocalizacao(String localizacao) {
        this.localizacao = localizacao;
    }

    public String getFotoCard() {
        return fotoCard;
    }

    public void setFotoCard(String fotoCard) {
        this.fotoCard = fotoCard;
    }

    public String getFotoPerfil() {
        return fotoPerfil;
    }

    public void setFotoPerfil(String fotoPerfil) {
        this.fotoPerfil = fotoPerfil;
    }

    public List<String> getCuidados() {
        return cuidados;
    }

    public void setCuidados(List<String> cuidados) {
        this.cuidados = cuidados;
    }

    public String getResumoCard() {
        return idade + " · " + sexo + " · " + porte;
    }

    public String getResumoPerfil() {
        return especie + " · " + sexo + " · " + idade + " · " + porte;
    }
}
