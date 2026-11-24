
import { Component, inject, signal } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Router, RouterLink } from '@angular/router';
import { FormsModule } from '@angular/forms';

interface RespostaLogin {
  accessToken: string;
  expiresAt: string;
  user: {
    id: string;
    fullName: string;
    email: string;
    phone: string | null;
    role: string;
  };
}

@Component({
  selector: 'app-login',
  imports: [FormsModule, RouterLink],
  templateUrl: './login.html',
  styleUrl: './login.css'
})
export class Login {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  email = '';
  senha = '';

  readonly entrando = signal(false);
  readonly erro = signal('');

  entrar(): void {
    if (this.entrando()) {
      return;
    }

    this.erro.set('');

    if (!this.email.trim() || !this.senha) {
      this.erro.set('Preencha o e-mail e a senha.');
      return;
    }

    this.entrando.set(true);

    const dados = {
      email: this.email.trim(),
      password: this.senha
    };

    this.http.post<RespostaLogin>('/api/auth/login', dados).subscribe({
      next: (resposta) => {
        this.entrando.set(false);

        sessionStorage.setItem('ampara_token', resposta.accessToken);
        sessionStorage.setItem('ampara_usuario', JSON.stringify(resposta.user));
        sessionStorage.setItem('ampara_expiracao', resposta.expiresAt);

        if (
          resposta.user.role === 'Admin' ||
          resposta.user.role === 'SuperAdmin'
        ) {
          this.router.navigateByUrl('/admin/visao-geral');
        } else {
          this.router.navigateByUrl('/');
        }
      },
      error: (erro: HttpErrorResponse) => {
        this.entrando.set(false);

        if (erro.status === 400 || erro.status === 401) {
          this.erro.set('E-mail ou senha incorretos.');
        } else {
          this.erro.set('Não foi possível entrar. Tente novamente.');
        }
      }
    });
  }
}
