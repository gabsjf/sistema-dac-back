package br.org.patasdobem.controller;

import br.org.patasdobem.model.AdocaoRequest;
import br.org.patasdobem.model.DoacaoRequest;
import br.org.patasdobem.service.UsuarioAreaService;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;
import java.util.LinkedHashMap;
import java.util.Map;

@RestController
@RequestMapping("/api")
public class ApiPublicaController {

    private final UsuarioAreaService usuarioAreaService;

    public ApiPublicaController(UsuarioAreaService usuarioAreaService) {
        this.usuarioAreaService = usuarioAreaService;
    }

    @PostMapping("/doacoes")
    public ResponseEntity<Map<String, Object>> simularDoacao(@RequestBody DoacaoRequest request) {
        String nome = (request.getNome() == null || request.getNome().isBlank()) ? "Apoiador(a)" : request.getNome().trim();
        String valor = (request.getValor() == null || request.getValor().isBlank()) ? "R$ 100,00" : request.getValor().trim();
        String pagamento = (request.getFormaPagamento() == null || request.getFormaPagamento().isBlank()) ? "Pix" : request.getFormaPagamento().trim();
        String dataHora = LocalDateTime.now().format(DateTimeFormatter.ofPattern("dd/MM/yyyy HH:mm"));

        usuarioAreaService.registrarDoacao(valor, pagamento, dataHora);

        Map<String, Object> response = new LinkedHashMap<>();
        response.put("status", "SIMULADA");
        response.put("mensagem", "Doação demonstrativa registrada com sucesso! Obrigado, " + nome + ".");
        response.put("valor", valor);
        response.put("formaPagamento", pagamento);
        response.put("campanha", "Um pote cheio. Muitas vidas felizes.");
        response.put("dataHora", dataHora);
        return ResponseEntity.ok(response);
    }

    @PostMapping("/adocoes")
    public ResponseEntity<Map<String, Object>> solicitarAdocao(@RequestBody AdocaoRequest request) {
        String interessado = (request.getNomeInteressado() == null || request.getNomeInteressado().isBlank())
                ? "Interessado(a)" : request.getNomeInteressado().trim();
        String animal = (request.getAnimalNome() == null || request.getAnimalNome().isBlank())
                ? "animal" : request.getAnimalNome().trim();

        Map<String, Object> response = new LinkedHashMap<>();
        response.put("status", "RECEBIDA");
        response.put("mensagem", "Solicitação para conhecer " + animal + " enviada! Nossa equipe entrará em contato com " + interessado + ".");
        response.put("dataHora", LocalDateTime.now().format(DateTimeFormatter.ofPattern("dd/MM/yyyy HH:mm")));
        return ResponseEntity.ok(response);
    }
}
