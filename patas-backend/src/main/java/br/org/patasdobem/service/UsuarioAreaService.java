package br.org.patasdobem.service;

import br.org.patasdobem.model.Animal;
import br.org.patasdobem.model.SolicitacaoAdocao;
import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.concurrent.atomic.AtomicInteger;

@Service
public class UsuarioAreaService {

    private final Map<String, SolicitacaoAdocao> solicitacoes = new LinkedHashMap<>();
    private final List<Map<String, String>> doacoes = new ArrayList<>();
    private final AtomicInteger sequenciaCodigo = new AtomicInteger(48);

    public UsuarioAreaService() {
        // Doação demonstrativa inicial vazia ou pronta para registrar
    }

    public List<SolicitacaoAdocao> listarSolicitacoes() {
        return new ArrayList<>(solicitacoes.values());
    }

    public SolicitacaoAdocao criarSolicitacao(Animal animal, String nome, String email, String telefone,
                                               String moradia, String rotina, String experiencia) {
        int num = sequenciaCodigo.getAndIncrement();
        String codigo = String.format("AD-%04d", num);
        SolicitacaoAdocao s = new SolicitacaoAdocao(
                codigo,
                animal.getId(),
                animal.getNome(),
                nome,
                email,
                telefone,
                moradia,
                rotina,
                experiencia,
                "EM_ANALISE",
                "Em análise"
        );
        solicitacoes.put(codigo, s);
        return s;
    }

    public Optional<SolicitacaoAdocao> buscarSolicitacao(String codigo) {
        if (codigo == null) {
            return Optional.empty();
        }
        return Optional.ofNullable(solicitacoes.get(codigo.toUpperCase()));
    }

    public SolicitacaoAdocao obterOuCriarDemonstracao(String codigo, Animal animalPadrao) {
        String key = (codigo == null || codigo.isBlank()) ? "AD-0048" : codigo.toUpperCase();
        return solicitacoes.computeIfAbsent(key, k -> new SolicitacaoAdocao(
                k,
                animalPadrao.getId(),
                animalPadrao.getNome(),
                "Ana Souza",
                "ana.souza@exemplo.com",
                "(11) 99999-0123",
                "Apartamento próprio, com telas de proteção.",
                "Trabalho híbrido, com companhia em casa todos os dias.",
                "Já cuidei de cães e conheço a rotina de passeios.",
                "EM_ANALISE",
                "Em análise"
        ));
    }

    public void aprovarSolicitacao(String codigo) {
        buscarSolicitacao(codigo).ifPresent(s -> {
            s.setStatus("APROVADA");
            s.setStatusLabel("Aguardando seu aceite");
        });
    }

    public void confirmarAceiteTermo(String codigo) {
        buscarSolicitacao(codigo).ifPresent(s -> {
            s.setStatus("TERMO_ACEITO");
            s.setStatusLabel("Termo aceito");
        });
    }

    public void concluirAdocao(String codigo) {
        buscarSolicitacao(codigo).ifPresent(s -> {
            s.setStatus("CONCLUIDA");
            s.setStatusLabel("Adoção concluída");
        });
    }

    public void reprovarSolicitacao(String codigo) {
        buscarSolicitacao(codigo).ifPresent(s -> {
            s.setStatus("NAO_APROVADA");
            s.setStatusLabel("Não aprovada");
        });
    }

    public List<Map<String, String>> listarDoacoes() {
        return doacoes;
    }

    public void registrarDoacao(String valor, String formaPagamento, String dataHora) {
        Map<String, String> item = new LinkedHashMap<>();
        item.put("campanha", "Um pote cheio. Muitas vidas felizes.");
        item.put("valor", valor);
        item.put("formaPagamento", formaPagamento);
        item.put("dataHora", dataHora);
        item.put("status", "Demonstrativa");
        doacoes.add(0, item);
    }
}
