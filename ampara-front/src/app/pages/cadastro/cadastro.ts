
import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface RespostaCadastro {
  accessToken: string;
  expiresAt: string;
  user: {
    id: string;
    fullName: string;
    email: string;
    phone: string;
    role: string;
  };
}

@Component({
  selector: 'app-cadastro',
  imports: [FormsModule, RouterLink],
  templateUrl: './cadastro.html',
  styleUrl: './cadastro.css'
})
export class Cadastro {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  nomeCompleto = '';
  email = '';
  telefone = '';
  senha = '';

  readonly enviando = signal(false);
  readonly erro = signal('');
  readonly sucesso = signal('');

  cadastrar(): void {
    if (this.enviando()) {
      return;
    }

    this.erro.set('');
    this.sucesso.set('');

    if (
      !this.nomeCompleto.trim() ||
      !this.email.trim() ||
      !this.telefone.trim() ||
      !this.senha
    ) {
      this.erro.set('Preencha todos os campos.');
      return;
    }

    this.enviando.set(true);

    const dados = {
      fullName: this.nomeCompleto.trim(),
      email: this.email.trim(),
      phone: this.telefone.trim(),
      password: this.senha
    };

    this.http.post<RespostaCadastro>('/api/auth/register', dados).subscribe({
      next: () => {
        this.enviando.set(false);
        this.sucesso.set('Cadastro realizado com sucesso!');

        this.router.navigateByUrl('/');
      },
      error: (erro: HttpErrorResponse) => {
        this.enviando.set(false);

        if (erro.status === 409) {
          this.erro.set('Este e-mail já está cadastrado.');
        } else if (erro.status === 400) {
          this.erro.set('Confira os dados informados e tente novamente.');
        } else {
          this.erro.set('Não foi possível realizar o cadastro. Tente novamente.');
        }
      }
    });
  }
}
