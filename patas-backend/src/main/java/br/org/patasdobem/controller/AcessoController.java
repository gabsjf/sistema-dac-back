package br.org.patasdobem.controller;

import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.GetMapping;

@Controller
public class AcessoController {

    @GetMapping("/entrar")
    public String login() {
        return "acesso/login";
    }

    @GetMapping("/registrar")
    public String registrar() {
        return "acesso/registrar";
    }

    @GetMapping("/redefinir-senha")
    public String redefinirSenha() {
        return "acesso/redefinir-senha";
    }
}
