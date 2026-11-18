package br.org.patasdobem.controller;

import br.org.patasdobem.model.Animal;
import br.org.patasdobem.model.SolicitacaoAdocao;
import br.org.patasdobem.service.AdminService;
import br.org.patasdobem.service.AnimalService;
import br.org.patasdobem.service.UsuarioAreaService;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestParam;

import java.util.List;

@Controller
@RequestMapping("/admin")
public class AdminController {

    private final AdminService adminService;
    private final AnimalService animalService;
    private final UsuarioAreaService usuarioAreaService;

    public AdminController(AdminService adminService, AnimalService animalService, UsuarioAreaService usuarioAreaService) {
        this.adminService = adminService;
        this.animalService = animalService;
        this.usuarioAreaService = usuarioAreaService;
    }

    private void addCommon(Model model, String menuAtivo) {
        model.addAttribute("menuAtivo", menuAtivo);
        model.addAttribute("pendentesCount", 8);
    }

    // A01 · Visão geral
    @GetMapping
    public String visaoGeral(@RequestParam(required = false) String salvo, Model model) {
        addCommon(model, "visao-geral");
        model.addAttribute("animais", adminService.listarAnimaisInternos().subList(0, 4));
        model.addAttribute("agenda", adminService.listarAgenda());
        model.addAttribute("campanha", adminService.getCampanha());
        model.addAttribute("salvo", salvo != null);
        return "admin/visao-geral";
    }

    // A02 · Animais
    @GetMapping("/animais")
    public String animais(@RequestParam(required = false) String salvo, Model model) {
        addCommon(model, "animais");
        model.addAttribute("animais", adminService.listarAnimaisInternos());
        model.addAttribute("salvo", salvo != null);
        return "admin/animais";
    }

    // A09 · Cadastrar animal
    @GetMapping("/animais/novo")
    public String novoAnimal(Model model) {
        addCommon(model, "animais");
        model.addAttribute("modoEdicao", false);
        model.addAttribute("animal", adminService.buscarAnimalPorId("luna"));
        return "admin/animal-form";
    }

    // A09 · Editar animal
    @GetMapping("/animais/{id}/editar")
    public String editarAnimal(@PathVariable String id, Model model) {
        addCommon(model, "animais");
        model.addAttribute("modoEdicao", true);
        model.addAttribute("animal", adminService.buscarAnimalPorId(id));
        return "admin/animal-form";
    }

    @PostMapping("/animais/salvar")
    public String salvarAnimal(@RequestParam(required = false) String id,
                               @RequestParam String nome,
                               @RequestParam String especieSexo,
                               @RequestParam String idade,
                               @RequestParam String porte,
                               @RequestParam String situacao,
                               @RequestParam(required = false, defaultValue = "") String observacoes) {
        adminService.salvarAnimal(id, nome, especieSexo, idade, porte, situacao, observacoes);
        return "redirect:/admin/animais?salvo=1";
    }

    // A15 · Prontuário do animal
    @GetMapping("/animais/{id}/prontuario")
    public String prontuario(@PathVariable String id, Model model) {
        addCommon(model, "animais");
        model.addAttribute("animal", adminService.buscarAnimalPorId(id));
        model.addAttribute("agenda", adminService.listarAgenda());
        return "admin/prontuario";
    }

    // A03 · Adoções
    @GetMapping("/adocoes")
    public String adocoes(@RequestParam(required = false) String salvo, Model model) {
        addCommon(model, "adocoes");
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        usuarioAreaService.obterOuCriarDemonstracao("AD-0048", luna);
        List<SolicitacaoAdocao> lista = usuarioAreaService.listarSolicitacoes();
        model.addAttribute("solicitacoes", lista);
        model.addAttribute("salvo", salvo != null);
        return "admin/adocoes";
    }

    // A10 · Analisar solicitação
    @GetMapping("/adocoes/{codigo}/analisar")
    public String analisarSolicitacao(@PathVariable String codigo, Model model) {
        addCommon(model, "adocoes");
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        model.addAttribute("solicitacao", usuarioAreaService.obterOuCriarDemonstracao(codigo, luna));
        return "admin/adocao-analisar";
    }

    // A18 · Agendar entrevista
    @GetMapping("/adocoes/{codigo}/entrevista")
    public String agendarEntrevista(@PathVariable String codigo, Model model) {
        addCommon(model, "adocoes");
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        model.addAttribute("solicitacao", usuarioAreaService.obterOuCriarDemonstracao(codigo, luna));
        return "admin/adocao-entrevista";
    }

    @PostMapping("/adocoes/{codigo}/entrevista")
    public String salvarEntrevista(@PathVariable String codigo) {
        return "redirect:/admin/adocoes/" + codigo + "/analisar";
    }

    // A11 · Termo e entrega
    @GetMapping("/adocoes/{codigo}/termo")
    public String termoEntrega(@PathVariable String codigo, Model model) {
        addCommon(model, "adocoes");
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        SolicitacaoAdocao s = usuarioAreaService.obterOuCriarDemonstracao(codigo, luna);
        usuarioAreaService.aprovarSolicitacao(s.getCodigo());
        model.addAttribute("solicitacao", s);
        return "admin/adocao-termo";
    }

    @PostMapping("/adocoes/{codigo}/concluir")
    public String concluirEntrega(@PathVariable String codigo) {
        usuarioAreaService.concluirAdocao(codigo);
        return "redirect:/admin/adocoes?salvo=1";
    }

    // A17 · Acompanhamento pós-adoção
    @GetMapping("/adocoes/{codigo}/acompanhamento")
    public String acompanhamento(@PathVariable String codigo, Model model) {
        addCommon(model, "adocoes");
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        model.addAttribute("solicitacao", usuarioAreaService.obterOuCriarDemonstracao(codigo, luna));
        return "admin/adocao-acompanhamento";
    }

    @PostMapping("/adocoes/{codigo}/acompanhamento")
    public String salvarAcompanhamento(@PathVariable String codigo) {
        return "redirect:/admin/adocoes?salvo=1";
    }

    // A19 · Encerrar solicitação
    @GetMapping("/adocoes/{codigo}/encerrar")
    public String encerrarSolicitacao(@PathVariable String codigo, Model model) {
        addCommon(model, "adocoes");
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        model.addAttribute("solicitacao", usuarioAreaService.obterOuCriarDemonstracao(codigo, luna));
        return "admin/adocao-encerrar";
    }

    @PostMapping("/adocoes/{codigo}/encerrar")
    public String confirmarEncerramento(@PathVariable String codigo) {
        usuarioAreaService.reprovarSolicitacao(codigo);
        return "redirect:/admin/adocoes?salvo=1";
    }

    // A04 · Saúde e cuidados
    @GetMapping("/saude")
    public String saude(@RequestParam(required = false) String salvo, Model model) {
        addCommon(model, "saude");
        model.addAttribute("agenda", adminService.listarAgenda());
        model.addAttribute("salvo", salvo != null);
        return "admin/saude";
    }

    // A12 · Registrar atendimento
    @GetMapping("/saude/novo")
    public String novoAtendimento(Model model) {
        addCommon(model, "saude");
        model.addAttribute("animais", adminService.listarAnimaisInternos());
        return "admin/saude-form";
    }

    @PostMapping("/saude/novo")
    public String salvarAtendimento(@RequestParam String animal,
                                    @RequestParam String procedimento,
                                    @RequestParam String horario,
                                    @RequestParam String profissional,
                                    @RequestParam String local) {
        adminService.registrarAtendimento(animal, procedimento, horario, profissional, local);
        return "redirect:/admin/saude?salvo=1";
    }

    // A05 · Financeiro
    @GetMapping("/financeiro")
    public String financeiro(@RequestParam(required = false) String salvo, Model model) {
        addCommon(model, "financeiro");
        model.addAttribute("lancamentos", adminService.listarLancamentos());
        model.addAttribute("campanha", adminService.getCampanha());
        model.addAttribute("salvo", salvo != null);
        return "admin/financeiro";
    }

    // A13 · Registrar lançamento financeiro
    @GetMapping("/financeiro/novo")
    public String novoLancamento(Model model) {
        addCommon(model, "financeiro");
        return "admin/financeiro-form";
    }

    @PostMapping("/financeiro/novo")
    public String salvarLancamento(@RequestParam String descricao,
                                   @RequestParam String tipo,
                                   @RequestParam String valor,
                                   @RequestParam String origem) {
        adminService.registrarLancamento(descricao, tipo, valor, origem);
        return "redirect:/admin/financeiro?salvo=1";
    }

    // A16 · Editar campanha
    @GetMapping("/financeiro/campanha")
    public String editarCampanha(Model model) {
        addCommon(model, "financeiro");
        model.addAttribute("campanha", adminService.getCampanha());
        return "admin/campanha-form";
    }

    @PostMapping("/financeiro/campanha")
    public String salvarCampanha(@RequestParam String titulo,
                                 @RequestParam String subtitulo,
                                 @RequestParam String descricao,
                                 @RequestParam String meta,
                                 @RequestParam String prazo) {
        adminService.atualizarCampanha(titulo, subtitulo, descricao, meta, prazo);
        return "redirect:/admin/financeiro?salvo=1";
    }

    // A06 · Voluntários
    @GetMapping("/voluntarios")
    public String voluntarios(@RequestParam(required = false) String salvo, Model model) {
        addCommon(model, "voluntarios");
        model.addAttribute("voluntarios", adminService.listarVoluntarios());
        model.addAttribute("salvo", salvo != null);
        return "admin/voluntarios";
    }

    // A14 · Cadastrar voluntário
    @GetMapping("/voluntarios/novo")
    public String novoVoluntario(Model model) {
        addCommon(model, "voluntarios");
        return "admin/voluntario-form";
    }

    @PostMapping("/voluntarios/novo")
    public String salvarVoluntario(@RequestParam String nome,
                                   @RequestParam String frente,
                                   @RequestParam String disponibilidade,
                                   @RequestParam String email) {
        adminService.cadastrarVoluntario(nome, frente, disponibilidade, email);
        return "redirect:/admin/voluntarios?salvo=1";
    }

    // A07 · Relatórios
    @GetMapping("/relatorios")
    public String relatorios(Model model) {
        addCommon(model, "relatorios");
        model.addAttribute("campanha", adminService.getCampanha());
        return "admin/relatorios";
    }

    // A08 · Configurações
    @GetMapping("/configuracoes")
    public String configuracoes(@RequestParam(required = false) String salvo, Model model) {
        addCommon(model, "configuracoes");
        model.addAttribute("config", adminService.getConfiguracoes());
        model.addAttribute("salvo", salvo != null);
        return "admin/configuracoes";
    }

    @PostMapping("/configuracoes")
    public String salvarConfiguracoes(@RequestParam String nomeOng,
                                      @RequestParam String cidade,
                                      @RequestParam String email,
                                      @RequestParam String catalogoPublico) {
        adminService.atualizarConfiguracoes(nomeOng, cidade, email, catalogoPublico);
        return "redirect:/admin/configuracoes?salvo=1";
    }
}
