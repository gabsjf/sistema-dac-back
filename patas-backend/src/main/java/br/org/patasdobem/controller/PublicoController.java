package br.org.patasdobem.controller;

import br.org.patasdobem.model.Animal;
import br.org.patasdobem.service.AnimalService;
import org.springframework.http.HttpStatus;
import org.springframework.stereotype.Controller;
import org.springframework.ui.Model;
import org.springframework.web.bind.annotation.GetMapping;
import org.springframework.web.bind.annotation.PathVariable;
import org.springframework.web.server.ResponseStatusException;

@Controller
public class PublicoController {

    private final AnimalService animalService;

    public PublicoController(AnimalService animalService) {
        this.animalService = animalService;
    }

    @GetMapping("/")
    public String inicio(Model model) {
        model.addAttribute("animais", animalService.listarTodos());
        model.addAttribute("totalAcolhidos", animalService.getTotalAnimaisAcolhidos());
        model.addAttribute("adocoesMes", animalService.getAdocoesNesteMes());
        return "publico/inicio";
    }

    @GetMapping("/animais/{id}")
    public String perfilAnimal(@PathVariable String id, Model model) {
        Animal animal = animalService.buscarPorId(id)
                .orElseThrow(() -> new ResponseStatusException(HttpStatus.NOT_FOUND, "Animal não encontrado"));
        model.addAttribute("animal", animal);
        return "publico/perfil-animal";
    }

    @GetMapping("/mapa")
    public String mapaFluxo() {
        return "publico/mapa";
    }
}
