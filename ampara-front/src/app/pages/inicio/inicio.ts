
import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';

interface Animal {
  id: string;
  name: string;
  species: string;
  sex: string;
  size: string;
  approximateAgeMonths: number;
  intakeDate: string;
  description: string;
  isVaccinated: boolean;
  isNeutered: boolean;
  isDewormed: boolean;
  photoUrl: string | null;
  status: string;
}

interface UsuarioSessao {
  id: string;
  fullName: string;
  email: string;
  phone: string;
  role: string;
}

@Component({
  selector: 'app-inicio',
  imports: [RouterLink],
  templateUrl: './inicio.html',
  styleUrl: './inicio.css'
})
export class Inicio implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  readonly animais = signal<Animal[]>([]);
  readonly carregando = signal(true);
  readonly erro = signal('');

  readonly usuarioLogado = signal<UsuarioSessao | null>(
    this.lerUsuario()
  );

  private lerUsuario(): UsuarioSessao | null {
    const usuario = sessionStorage.getItem('ampara_usuario');

    if (!usuario) {
      return null;
    }

    try {
      return JSON.parse(usuario) as UsuarioSessao;
    } catch {
      sessionStorage.removeItem('ampara_usuario');
      return null;
    }
  }

  ngOnInit(): void {
    this.http.get<Animal[]>('/api/public/animals').subscribe({
      next: (dados) => {
        this.animais.set(dados);
        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os animais.');
        this.carregando.set(false);
      }
    });
  }

  sair(): void {
    sessionStorage.removeItem('ampara_token');
    sessionStorage.removeItem('ampara_usuario');
    sessionStorage.removeItem('ampara_expiracao');

    this.usuarioLogado.set(null);

    this.router.navigateByUrl('/');
  }

  imagemAnimal(animal: Animal): string {
    if (animal.photoUrl) {
      return animal.photoUrl;
    }

    const imagens: Record<string, string> = {
      luna: '/assets/images/Luna-card.svg',
      chico: '/assets/images/Chico-card.svg'
    };

    return imagens[animal.name.toLowerCase()] ??
      '/assets/images/Nina-card.svg';
  }

  especie(animal: Animal): string {
    const especies: Record<string, string> = {
      Canine: 'Cachorro',
      Feline: 'Gato'
    };

    return especies[animal.species] ?? animal.species;
  }

  sexo(animal: Animal): string {
    const sexos: Record<string, string> = {
      Female: 'Fêmea',
      Male: 'Macho'
    };

    return sexos[animal.sex] ?? animal.sex;
  }

  porte(animal: Animal): string {
    const portes: Record<string, string> = {
      Small: 'Pequeno',
      Medium: 'Médio',
      Large: 'Grande'
    };

    return portes[animal.size] ?? animal.size;
  }

  idade(animal: Animal): string {
    const meses = animal.approximateAgeMonths;

    if (meses < 12) {
      return `${meses} meses`;
    }

    const anos = Math.floor(meses / 12);
    const restantes = meses % 12;

    if (restantes === 0) {
      return `${anos} ${anos === 1 ? 'ano' : 'anos'}`;
    }

    return `${anos} ${anos === 1 ? 'ano' : 'anos'} e ${restantes} meses`;
  }
}
