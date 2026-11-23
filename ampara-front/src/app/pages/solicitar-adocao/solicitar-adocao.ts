
import { Component, OnInit, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface Animal {
  id: string;
  name: string;
  species: string;
  sex: string;
  size: string;
  approximateAgeMonths: number;
  description: string;
  photoUrl: string | null;
  status: string;
}

interface SolicitacaoResposta {
  id: string;
  animalId: string;
  status: string;
}

@Component({
  selector: 'app-solicitar-adocao',
  imports: [FormsModule, RouterLink],
  templateUrl: './solicitar-adocao.html',
  styleUrl: './solicitar-adocao.css'
})
export class SolicitarAdocao implements OnInit {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);

  readonly animal = signal<Animal | null>(null);
  readonly carregando = signal(true);
  readonly enviando = signal(false);
  readonly erro = signal('');

  telefone = '';
  moradia = '';
  rotina = '';
  mensagem = '';
  aceitaTermos = false;

  ngOnInit(): void {
    if (!sessionStorage.getItem('ampara_token')) {
      this.router.navigate(['/entrar']);
      return;
    }

    const animalId = this.route.snapshot.paramMap.get('id');

    if (!animalId) {
      this.erro.set('Animal não identificado.');
      this.carregando.set(false);
      return;
    }

    this.http.get<Animal[]>('/api/public/animals').subscribe({
      next: (animais) => {
        const encontrado = animais.find(
          item => item.id === animalId
        );

        if (!encontrado) {
          this.erro.set('Animal não encontrado.');
        } else if (encontrado.status !== 'Available') {
          this.erro.set('Este animal não está disponível para adoção.');
        } else {
          this.animal.set(encontrado);
        }

        this.carregando.set(false);
      },
      error: () => {
        this.erro.set('Não foi possível carregar os dados do animal.');
        this.carregando.set(false);
      }
    });
  }

  enviarSolicitacao(): void {
    const pet = this.animal();

    if (!pet || this.enviando()) {
      return;
    }

    if (
      !this.telefone.trim() ||
      !this.moradia.trim() ||
      !this.rotina.trim() ||
      !this.mensagem.trim() ||
      !this.aceitaTermos
    ) {
      this.erro.set('Preencha todos os campos e aceite os termos.');
      return;
    }

    this.erro.set('');
    this.enviando.set(true);

    const dados = {
      animalId: pet.id,
      phone: this.telefone.trim(),
      housing: this.moradia.trim(),
      routine: this.rotina.trim(),
      message: this.mensagem.trim(),
      acceptsTerms: this.aceitaTermos
    };

    this.http.post('/api/adoptions', dados).subscribe({
      next: () => {
        this.enviando.set(false);
        this.router.navigateByUrl('/');
      },
      error: (erro: HttpErrorResponse) => {
        this.enviando.set(false);

        if (erro.status === 401 || erro.status === 403) {
          this.erro.set('Sua sessão expirou. Entre novamente.');
          this.router.navigateByUrl('/entrar');
        } else if (erro.status === 400) {
          this.erro.set('Confira os dados informados e tente novamente.');
        } else {
          this.erro.set('Não foi possível enviar a solicitação.');
        }
      }
    });
  }
}
