package br.org.patasdobem.service;

import br.org.patasdobem.model.Animal;
import org.springframework.stereotype.Service;

import java.util.Arrays;
import java.util.LinkedHashMap;
import java.util.List;
import java.util.Map;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
public class AnimalService {

    private final Map<String, Animal> catalogo = new LinkedHashMap<>();

    public AnimalService() {
        List<String> cuidadosPadrao = Arrays.asList(
                "Castração realizada",
                "Cuidados veterinários acompanhados",
                "Avaliação de temperamento"
        );

        catalogo.put("luna", new Animal(
                "luna",
                "Luna",
                "Canina",
                "Fêmea",
                "2 anos",
                "Porte médio",
                "Disponível",
                "Disponível para adoção",
                "Carinhosa e companheira. Luna adora companhia e passeios tranquilos.",
                "São Paulo, SP · ONG Patas do Bem",
                "/assets/images/Luna-card.svg",
                "/assets/images/Luna.svg",
                cuidadosPadrao
        ));

        catalogo.put("nina", new Animal(
                "nina",
                "Nina",
                "Felina",
                "Fêmea",
                "1 ano",
                "Porte pequeno",
                "Disponível",
                "Disponível para adoção",
                "Curiosa, dócil e muito tranquila. Nina adora lugares aconchegantes e carinho no queixo.",
                "São Paulo, SP · ONG Patas do Bem",
                "/assets/images/Nina-card.svg",
                "/assets/images/Nina-card.svg",
                cuidadosPadrao
        ));

        catalogo.put("chico", new Animal(
                "chico",
                "Chico",
                "Canina",
                "Macho",
                "3 anos",
                "Porte médio",
                "Disponível",
                "Disponível para adoção",
                "Leal, brincalhão e super amigável. Chico se dá muito bem com famílias e adora áreas abertas.",
                "São Paulo, SP · ONG Patas do Bem",
                "/assets/images/Chico-card.svg",
                "/assets/images/Chico-card.svg",
                cuidadosPadrao
        ));
    }

    public List<Animal> listarTodos() {
        return catalogo.values().stream().collect(Collectors.toList());
    }

    public Optional<Animal> buscarPorId(String id) {
        if (id == null) {
            return Optional.empty();
        }
        return Optional.ofNullable(catalogo.get(id.toLowerCase()));
    }

    public int getTotalAnimaisAcolhidos() {
        return 128;
    }

    public int getAdocoesNesteMes() {
        return 24;
    }
}
