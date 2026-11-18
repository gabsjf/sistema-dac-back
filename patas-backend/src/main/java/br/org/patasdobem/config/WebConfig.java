package br.org.patasdobem.config;

import org.springframework.context.annotation.Bean;
import org.springframework.context.annotation.Configuration;
import org.springframework.web.servlet.config.annotation.ResourceHandlerRegistry;
import org.springframework.web.servlet.config.annotation.WebMvcConfigurer;
import org.thymeleaf.spring6.templateresolver.SpringResourceTemplateResolver;
import org.thymeleaf.templatemode.TemplateMode;

import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

@Configuration
public class WebConfig implements WebMvcConfigurer {

    private Path resolveFrontendDir() {
        Path direct = Paths.get("patas-frontend").toAbsolutePath().normalize();
        if (Files.isDirectory(direct)) {
            return direct;
        }
        return Paths.get("..", "patas-frontend").toAbsolutePath().normalize();
    }

    @Override
    public void addResourceHandlers(ResourceHandlerRegistry registry) {
        String staticUri = resolveFrontendDir().resolve("static").toUri().toString();
        registry.addResourceHandler("/**")
                .addResourceLocations(staticUri)
                .setCachePeriod(0);
    }

    @Bean
    public SpringResourceTemplateResolver templateResolver() {
        SpringResourceTemplateResolver resolver = new SpringResourceTemplateResolver();
        String templatesUri = resolveFrontendDir().resolve("templates").toUri().toString();
        if (!templatesUri.endsWith("/")) {
            templatesUri += "/";
        }
        resolver.setPrefix(templatesUri);
        resolver.setSuffix(".html");
        resolver.setTemplateMode(TemplateMode.HTML);
        resolver.setCharacterEncoding("UTF-8");
        resolver.setCacheable(false);
        return resolver;
    }
}
