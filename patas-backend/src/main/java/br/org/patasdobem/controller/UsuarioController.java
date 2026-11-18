package br.org.patasdobem.controller;

import br.org.patasdobem.model.Animal;
import br.org.patasdobem.model.SolicitacaoAdocao;
import br.org.patasdobem.service.AnimalService;
import br.org.patasdobem.service.UsuarioAreaService;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.server.ResponseStatusException;

@Controller
public class UsuarioController {

    private final AnimalService animalService;
    private final UsuarioAreaService usuarioAreaService;

    public UsuarioController(AnimalService animalService, UsuarioAreaService usuarioAreaService) {
        this.animalService = animalService;
        this.usuarioAreaService = usuarioAreaService;
    }

    @GetMapping("/minha-area")
    public String minhaArea(@RequestParam(name = "aba", defaultValue = "adocoes") String aba, Model model) {
        model.addAttribute("abaAtiva", aba);
        model.addAttribute("solicitacoes", usuarioAreaService.listarSolicitacoes());
        model.addAttribute("doacoes", usuarioAreaService.listarDoacoes());
        return "usuario/minha-area";
    }

    @GetMapping("/solicitar-adocao/{animalId}")
    public String solicitarAdocaoForm(@PathVariable String animalId, Model model) {
        Animal animal = animalService.buscarPorId(animalId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Animal não encontrado"));
        model.addAttribute("animal", animal);
        return "usuario/solicitar-adocao";
    }

    @PostMapping("/solicitar-adocao/{animalId}")
    public String enviarSolicitacao(@PathVariable String animalId,
                                    @RequestParam String nome,
                                    @RequestParam String email,
                                    @RequestParam String telefone,
                                    @RequestParam String moradia,
                                    @RequestParam String rotina,
                                    @RequestParam(required = false, defaultValue = "") String experiencia) {
        Animal animal = animalService.buscarPorId(animalId)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Animal não encontrado"));
        SolicitacaoAdocao criada = usuarioAreaService.criarSolicitacao(
                animal, nome, email, telefone, moradia, rotina, experiencia
        );
        return "redirect:/minha-area/solicitacao/" + criada.getCodigo();
    }

    @GetMapping("/minha-area/solicitacao/{codigo}")
    public String solicitacaoEmAnalise(@PathVariable String codigo, Model model) {
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        SolicitacaoAdocao solicitacao = usuarioAreaService.obterOuCriarDemonstracao(codigo, luna);
        model.addAttribute("solicitacao", solicitacao);
        return "usuario/solicitacao-analise";
    }

    @GetMapping("/minha-area/solicitacao/{codigo}/aprovada")
    public String solicitacaoAprovada(@PathVariable String codigo, Model model) {
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        SolicitacaoAdocao solicitacao = usuarioAreaService.obterOuCriarDemonstracao(codigo, luna);
        usuarioAreaService.aprovarSolicitacao(solicitacao.getCodigo());
        model.addAttribute("solicitacao", solicitacao);
        return "usuario/solicitacao-aprovada";
    }

    @GetMapping("/minha-area/solicitacao/{codigo}/termo")
    public String termoAdocao(@PathVariable String codigo, Model model) {
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        SolicitacaoAdocao solicitacao = usuarioAreaService.obterOuCriarDemonstracao(codigo, luna);
        model.addAttribute("solicitacao", solicitacao);
        return "usuario/termo-adocao";
    }

    @PostMapping("/minha-area/solicitacao/{codigo}/termo")
    public String confirmarAceiteTermo(@PathVariable String codigo) {
        usuarioAreaService.confirmarAceiteTermo(codigo);
        return "redirect:/minha-area/solicitacao/" + codigo + "/concluida";
    }

    @GetMapping("/minha-area/solicitacao/{codigo}/concluida")
    public String adocaoConcluida(@PathVariable String codigo, Model model) {
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        SolicitacaoAdocao solicitacao = usuarioAreaService.obterOuCriarDemonstracao(codigo, luna);
        usuarioAreaService.concluirAdocao(solicitacao.getCodigo());
        model.addAttribute("solicitacao", solicitacao);
        return "usuario/solicitacao-concluida";
    }

    @GetMapping("/minha-area/solicitacao/{codigo}/nao-aprovada")
    public String solicitacaoNaoAprovada(@PathVariable String codigo, Model model) {
        Animal luna = animalService.buscarPorId("luna").orElseThrow();
        SolicitacaoAdocao solicitacao = usuarioAreaService.obterOuCriarDemonstracao(codigo, luna);
        usuarioAreaService.reprovarSolicitacao(solicitacao.getCodigo());
        model.addAttribute("solicitacao", solicitacao);
        return "usuario/solicitacao-nao-aprovada";
    }
}
