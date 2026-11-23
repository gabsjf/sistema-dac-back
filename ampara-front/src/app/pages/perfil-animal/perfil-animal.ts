
import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute, RouterLink } from '@angular/router';

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

@Component({
  selector: 'app-perfil-animal',
  imports: [RouterLink],
  templateUrl: './perfil-animal.html',
  styleUrl: './perfil-animal.css'
})
export class PerfilAnimal implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);

  readonly animal = signal<Animal | null>(null);
  readonly carregando = signal(true);
  readonly erro = signal('');

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');

    if (!id) {
      this.erro.set('Animal não encontrado.');
      this.carregando.set(false);
      return;
    }

    this.http.get<Animal[]>('/api/public/animals').subscribe({
      next: (animais) => {
        const encontrado = animais.find(item => item.id === id);

        if (encontrado) {
          this.animal.set(encontrado);
        } else {
          this.erro.set('Animal não encontrado.');
        }

        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar o perfil do animal.');
        this.carregando.set(false);
      }
    });
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
