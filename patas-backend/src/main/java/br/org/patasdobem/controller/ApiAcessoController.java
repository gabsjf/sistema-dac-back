package br.org.patasdobem.controller;

import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.PostMapping;
import org.springframework.web.bind.annotation.RequestBody;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;

import java.util.LinkedHashMap;
import java.util.Map;

@RestController
@RequestMapping("/api/auth")
public class ApiAcessoController {

    @PostMapping("/login")
    public ResponseEntity<Map<String, Object>> login(@RequestBody Map<String, String> payload) {
        String email = payload.getOrDefault("email", "").trim();
        String perfil = payload.getOrDefault("perfil", "usuario");

        Map<String, Object> response = new LinkedHashMap<>();
        if (email.isEmpty()) {
            response.put("ok", false);
            response.put("mensagem", "Informe seu e-mail para entrar.");
            return ResponseEntity.badRequest().body(response);
        }

        String labelPerfil = "admin".equalsIgnoreCase(perfil) ? "Administrador" : "Usuário";
        response.put("ok", true);
        response.put("perfil", perfil);
        response.put("mensagem", "Login realizado como " + labelPerfil + " (" + email + ").");
        return ResponseEntity.ok(response);
    }

    @PostMapping("/registrar")
    public ResponseEntity<Map<String, Object>> registrar(@RequestBody Map<String, Object> payload) {
        String nome = String.valueOf(payload.getOrDefault("nome", "")).trim();
        String email = String.valueOf(payload.getOrDefault("email", "")).trim();
        String senha = String.valueOf(payload.getOrDefault("senha", ""));
        String confirmar = String.valueOf(payload.getOrDefault("confirmarSenha", ""));
        boolean termos = Boolean.parseBoolean(String.valueOf(payload.getOrDefault("termos", false)));

        Map<String, Object> response = new LinkedHashMap<>();
        if (nome.isEmpty() || email.isEmpty()) {
            response.put("ok", false);
            response.put("mensagem", "Preencha nome completo e e-mail.");
            return ResponseEntity.badRequest().body(response);
        }
        if (senha.length() < 8) {
            response.put("ok", false);
            response.put("mensagem", "A senha deve ter pelo menos 8 caracteres.");
            return ResponseEntity.badRequest().body(response);
        }
        if (!senha.equals(confirmar)) {
            response.put("ok", false);
            response.put("mensagem", "As senhas informadas não coincidem.");
            return ResponseEntity.badRequest().body(response);
        }
        if (!termos) {
            response.put("ok", false);
            response.put("mensagem", "É necessário concordar com os Termos de uso e a Privacidade.");
            return ResponseEntity.badRequest().body(response);
        }

        response.put("ok", true);
        response.put("mensagem", "Conta de usuário criada para " + nome + " (" + email + ")!");
        return ResponseEntity.ok(response);
    }

    @PostMapping("/recuperar")
    public ResponseEntity<Map<String, Object>> recuperar(@RequestBody Map<String, String> payload) {
        String email = payload.getOrDefault("email", "").trim();
        Map<String, Object> response = new LinkedHashMap<>();
        if (email.isEmpty()) {
            response.put("ok", false);
            response.put("mensagem", "Informe o e-mail da sua conta.");
            return ResponseEntity.badRequest().body(response);
        }
        response.put("ok", true);
        response.put("mensagem", "Se o e-mail " + email + " estiver cadastrado, você receberá as orientações de acesso.");
        return ResponseEntity.ok(response);
    }

    @PostMapping("/redefinir")
    public ResponseEntity<Map<String, Object>> redefinir(@RequestBody Map<String, String> payload) {
        String senha = payload.getOrDefault("novaSenha", "");
        String confirmar = payload.getOrDefault("confirmarNovaSenha", "");

        Map<String, Object> response = new LinkedHashMap<>();
        if (senha.length() < 8 || !senha.matches(".*[A-Za-z].*") || !senha.matches(".*\\d.*")) {
            response.put("ok", false);
            response.put("mensagem", "Use pelo menos 8 caracteres, com letras e números.");
            return ResponseEntity.badRequest().body(response);
        }
        if (!senha.equals(confirmar)) {
            response.put("ok", false);
            response.put("mensagem", "As senhas informadas não coincidem.");
            return ResponseEntity.badRequest().body(response);
        }

        response.put("ok", true);
        response.put("mensagem", "Nova senha salva com sucesso! Você já pode entrar na sua conta.");
        return ResponseEntity.ok(response);
    }
}
