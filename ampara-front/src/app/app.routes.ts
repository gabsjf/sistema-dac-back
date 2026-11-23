
import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pages/inicio/inicio').then(m => m.Inicio)
  },
  {
    path: 'entrar',
    loadComponent: () =>
      import('./pages/login/login').then(m => m.Login)
  },
  {
    path: 'registrar',
    loadComponent: () =>
      import('./pages/cadastro/cadastro').then(m => m.Cadastro)
  },
  {
    path: 'minhas-solicitacoes',
    loadComponent: () =>
      import('./pages/minhas-solicitacoes/minhas-solicitacoes').then(
        m => m.MinhasSolicitacoes
      )
  },
  {
    path: 'animais/:id/adotar',
    loadComponent: () =>
      import('./pages/solicitar-adocao/solicitar-adocao').then(
        m => m.SolicitarAdocao
      )
  },
  {
    path: 'animais/:id',
    loadComponent: () =>
      import('./pages/perfil-animal/perfil-animal').then(
        m => m.PerfilAnimal
      )
  },
  {
    path: 'admin/adocoes',
    loadComponent: () =>
      import('./pages/staff/adocoes/adocoes').then(m => m.Adocoes),
    canActivate: [() => {
      const usuario = sessionStorage.getItem('ampara_usuario');

      if (!usuario) {
        window.location.href = '/entrar';
        return false;
      }

      try {
        const dados = JSON.parse(usuario);

        if (dados.role === 'Admin' || dados.role === 'SuperAdmin') {
          return true;
        }
      } catch {
        // Dados de sessão inválidos.
      }

      window.location.href = '/';
      return false;
    }]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
