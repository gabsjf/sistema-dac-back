package br.org.patasdobem.service;

import org.springframework.stereotype.Service;

import java.util.ArrayList;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;

@Service
public class AdminService {

    private final List<Map<String, String>> animaisInternos = new ArrayList<>();
    private final List<Map<String, String>> agendaCuidados = new ArrayList<>();
    private final List<Map<String, String>> lancamentosFinanceiros = new ArrayList<>();
    private final List<Map<String, String>> voluntarios = new ArrayList<>();
    private final Map<String, String> campanha = new LinkedHashMap<>();
    private final Map<String, String> configuracoes = new LinkedHashMap<>();

    public AdminService() {
        animaisInternos.add(criarAnimal("AN-0128", "luna", "Luna", "Canina, fêmea", "2 anos", "Médio", "Disponível", "disponivel", "16 nov, 2026", "Vacinada, castrada e sociável. Ótima com famílias.", "Mariana Costa"));
        animaisInternos.add(criarAnimal("AN-0127", "thor", "Thor", "Canino, macho", "4 anos", "Grande", "Em tratamento", "tratamento", "15 nov, 2026", "Em recuperação dermatológica. Retorno com Dr. Rafael.", "Rafael Lima"));
        animaisInternos.add(criarAnimal("AN-0126", "mel", "Mel", "Felina, fêmea", "1 ano", "Pequeno", "Em tratamento", "tratamento", "14 nov, 2026", "Pós-castração agendada com Dra. Camila.", "Camila Rocha"));
        animaisInternos.add(criarAnimal("AN-0125", "bento", "Bento", "Canino, macho", "3 anos", "Pequeno", "Em adaptação", "adaptacao", "12 nov, 2026", "Em período de adaptação com a família acolhedora.", "Mariana Costa"));
        animaisInternos.add(criarAnimal("AN-0124", "nina", "Nina", "Felina, fêmea", "1 ano", "Pequeno", "Disponível", "disponivel", "10 nov, 2026", "Nina é dócil, curiosa e gosta de companhia.", "Mariana Costa"));
        animaisInternos.add(criarAnimal("AN-0123", "chico", "Chico", "Canino, macho", "3 anos", "Médio", "Disponível", "disponivel", "08 nov, 2026", "Brincalhão, castrado e ótimo para passeios.", "Rafael Lima"));

        agendaCuidados.add(criarAtendimento("AT-01", "09:00", "Vacina múltipla · Luna", "Dra. Camila • Consultório 1", "mint", "18 nov, 2026", "Concluído"));
        agendaCuidados.add(criarAtendimento("AT-02", "10:30", "Retorno clínico · Thor", "Dr. Rafael • Consultório 2", "amber", "18 nov, 2026", "Em andamento"));
        agendaCuidados.add(criarAtendimento("AT-03", "14:00", "Castração · Mel", "Dra. Camila • Centro cirúrgico", "violet", "18 nov, 2026", "Agendado"));

        lancamentosFinanceiros.add(criarLancamento("FN-041", "Doação · Mariana Costa", "Entrada", "+ R$ 250,00", "18 nov, 2026", "Doação", "Recebido"));
        lancamentosFinanceiros.add(criarLancamento("FN-040", "Compra de ração", "Despesa", "- R$ 620,00", "17 nov, 2026", "Alimentação", "Pago"));
        lancamentosFinanceiros.add(criarLancamento("FN-039", "Consulta e exames · Thor", "Despesa", "- R$ 380,00", "16 nov, 2026", "Saúde veterinária", "Pago"));
        lancamentosFinanceiros.add(criarLancamento("FN-038", "Doação · João Pedro", "Entrada", "+ R$ 150,00", "15 nov, 2026", "Doação", "Recebido"));
        lancamentosFinanceiros.add(criarLancamento("FN-037", "Transporte de resgate", "Despesa", "- R$ 90,00", "14 nov, 2026", "Transporte", "Pago"));

        voluntarios.add(criarVoluntario("VL-01", "Pedro Martins", "Passeios e socialização", "Sábados · Manhã", "Ativo", "pedro@exemplo.com"));
        voluntarios.add(criarVoluntario("VL-02", "Sofia Almeida", "Eventos e campanhas", "Domingos · Tarde", "Ativo", "sofia@exemplo.com"));
        voluntarios.add(criarVoluntario("VL-03", "Lucas Santos", "Transporte solidário", "Terças · Manhã", "Ativo", "lucas@exemplo.com"));
        voluntarios.add(criarVoluntario("VL-04", "Beatriz Lima", "Lar temporário", "A combinar", "Em integração", "beatriz@exemplo.com"));
        voluntarios.add(criarVoluntario("VL-05", "Gabriel Costa", "Apoio ao abrigo", "Sábados · Tarde", "Ativo", "gabriel@exemplo.com"));

        campanha.put("titulo", "Um pote cheio.");
        campanha.put("subtitulo", "Muitas vidas felizes.");
        campanha.put("descricao", "Ajude a garantir ração e cuidado para quem está esperando um novo lar.");
        campanha.put("arrecadado", "R$ 6.800");
        campanha.put("meta", "R$ 10.000");
        campanha.put("percentual", "68");
        campanha.put("apoiadores", "32 apoiadores");
        campanha.put("prazo", "Encerra em 12 dias");

        configuracoes.put("nomeOng", "ONG Patas do Bem");
        configuracoes.put("cidade", "São Paulo, SP");
        configuracoes.put("email", "contato@patasdobem.org.br");
        configuracoes.put("plano", "Plano Essencial");
        configuracoes.put("catalogoPublico", "Ativo");
    }

    private Map<String, String> criarAnimal(String codigo, String id, String nome, String especieSexo,
                                            String idade, String porte, String situacao, String situacaoTipo,
                                            String data, String observacoes, String responsavel) {
        Map<String, String> m = new LinkedHashMap<>();
        m.put("codigo", codigo);
        m.put("id", id);
        m.put("nome", nome);
        m.put("especieSexo", especieSexo);
        m.put("idade", idade);
        m.put("porte", porte);
        m.put("situacao", situacao);
        m.put("situacaoTipo", situacaoTipo);
        m.put("data", data);
        m.put("observacoes", observacoes);
        m.put("responsavel", responsavel);
        return m;
    }

    private Map<String, String> criarAtendimento(String id, String horario, String titulo, String local,
                                                 String cor, String data, String status) {
        Map<String, String> m = new LinkedHashMap<>();
        m.put("id", id);
        m.put("horario", horario);
        m.put("titulo", titulo);
        m.put("local", local);
        m.put("cor", cor);
        m.put("data", data);
        m.put("status", status);
        return m;
    }

    private Map<String, String> criarLancamento(String id, String descricao, String tipo, String valor,
                                                String data, String origem, String situacao) {
        Map<String, String> m = new LinkedHashMap<>();
        m.put("id", id);
        m.put("descricao", descricao);
        m.put("tipo", tipo);
        m.put("valor", valor);
        m.put("data", data);
        m.put("origem", origem);
        m.put("situacao", situacao);
        return m;
    }

    private Map<String, String> criarVoluntario(String id, String nome, String frente, String disponibilidade,
                                                String status, String email) {
        Map<String, String> m = new LinkedHashMap<>();
        m.put("id", id);
        m.put("nome", nome);
        m.put("frente", frente);
        m.put("disponibilidade", disponibilidade);
        m.put("status", status);
        m.put("email", email);
        return m;
    }

    public List<Map<String, String>> listarAnimaisInternos() {
        return animaisInternos;
    }

    public Map<String, String> buscarAnimalPorId(String id) {
        return animaisInternos.stream()
                .filter(a -> a.get("id").equalsIgnoreCase(id))
                .findFirst()
                .orElse(animaisInternos.get(0));
    }

    public void salvarAnimal(String id, String nome, String especieSexo, String idade, String porte, String situacao, String observacoes) {
        String tipo = "Disponível".equalsIgnoreCase(situacao) ? "disponivel"
                : ("Em adaptação".equalsIgnoreCase(situacao) ? "adaptacao" : "tratamento");

        if (id != null && !id.isBlank()) {
            for (Map<String, String> a : animaisInternos) {
                if (a.get("id").equalsIgnoreCase(id)) {
                    a.put("nome", nome);
                    a.put("especieSexo", especieSexo);
                    a.put("idade", idade);
                    a.put("porte", porte);
                    a.put("situacao", situacao);
                    a.put("situacaoTipo", tipo);
                    a.put("observacoes", observacoes);
                    return;
                }
            }
        }
        String novoCodigo = String.format("AN-%04d", 129 + animaisInternos.size());
        String novoId = nome.toLowerCase().replaceAll("[^a-z0-9]", "");
        animaisInternos.add(0, criarAnimal(novoCodigo, novoId, nome, especieSexo, idade, porte, situacao, tipo, "18 nov, 2026", observacoes, "Mariana Costa"));
    }

    public List<Map<String, String>> listarAgenda() {
        return agendaCuidados;
    }

    public void registrarAtendimento(String animal, String procedimento, String horario, String profissional, String local) {
        String id = String.format("AT-%02d", agendaCuidados.size() + 1);
        agendaCuidados.add(0, criarAtendimento(id, horario, procedimento + " · " + animal, profissional + " • " + local, "mint", "18 nov, 2026", "Agendado"));
    }

    public List<Map<String, String>> listarLancamentos() {
        return lancamentosFinanceiros;
    }

    public void registrarLancamento(String descricao, String tipo, String valor, String origem) {
        String id = String.format("FN-%03d", 42 + lancamentosFinanceiros.size());
        String situacao = "Entrada".equalsIgnoreCase(tipo) ? "Recebido" : "Pago";
        String sinal = "Entrada".equalsIgnoreCase(tipo) ? "+ " : "- ";
        String valorFmt = valor.startsWith("+") || valor.startsWith("-") ? valor : sinal + valor;
        lancamentosFinanceiros.add(0, criarLancamento(id, descricao, tipo, valorFmt, "18 nov, 2026", origem, situacao));
    }

    public List<Map<String, String>> listarVoluntarios() {
        return voluntarios;
    }

    public void cadastrarVoluntario(String nome, String frente, String disponibilidade, String email) {
        String id = String.format("VL-%02d", voluntarios.size() + 1);
        voluntarios.add(0, criarVoluntario(id, nome, frente, disponibilidade, "Ativo", email));
    }

    public Map<String, String> getCampanha() {
        return campanha;
    }

    public void atualizarCampanha(String titulo, String subtitulo, String descricao, String meta, String prazo) {
        campanha.put("titulo", titulo);
        campanha.put("subtitulo", subtitulo);
        campanha.put("descricao", descricao);
        campanha.put("meta", meta);
        campanha.put("prazo", prazo);
    }

    public Map<String, String> getConfiguracoes() {
        return configuracoes;
    }

    public void atualizarConfiguracoes(String nomeOng, String cidade, String email, String catalogoPublico) {
        configuracoes.put("nomeOng", nomeOng);
        configuracoes.put("cidade", cidade);
        configuracoes.put("email", email);
        configuracoes.put("catalogoPublico", catalogoPublico);
    }
}
