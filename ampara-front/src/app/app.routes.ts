import { Routes } from '@angular/router';
import { adotanteGuard, equipeGuard, superAdminGuard } from './core/role.guard';

// Áreas do menu da equipe que ainda não têm funcionalidade nem API.
const emDesenvolvimento = (caminho: string, secao: string, descricao: string) => ({
  path: caminho,
  loadComponent: () =>
    import('./pages/staff/em-desenvolvimento/em-desenvolvimento').then(
      m => m.EmDesenvolvimento
    ),
  data: { secao, descricao }
});

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
      ),
    canActivate: [adotanteGuard]
  },
  {
    path: 'animais/:id/adotar',
    loadComponent: () =>
      import('./pages/solicitar-adocao/solicitar-adocao').then(
        m => m.SolicitarAdocao
      ),
    canActivate: [adotanteGuard]
  },
  {
    path: 'animais/:id',
    loadComponent: () =>
      import('./pages/perfil-animal/perfil-animal').then(
        m => m.PerfilAnimal
      )
  },
  {
    path: 'admin',
    loadComponent: () =>
      import('./pages/staff/shell/admin-shell').then(m => m.AdminShell),
    canActivate: [equipeGuard],
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'visao-geral' },
      {
        path: 'visao-geral',
        loadComponent: () =>
          import('./pages/staff/visao-geral/visao-geral').then(m => m.VisaoGeral)
      },
      {
        path: 'animais',
        loadComponent: () =>
          import('./pages/staff/animais/animais').then(m => m.Animais)
      },
      {
        path: 'adocoes',
        loadComponent: () =>
          import('./pages/staff/adocoes/adocoes').then(m => m.Adocoes)
      },
      {
        path: 'equipe',
        loadComponent: () =>
          import('./pages/staff/equipe/equipe').then(m => m.Equipe),
        canActivate: [superAdminGuard]
      },
      emDesenvolvimento(
        'saude',
        'Saúde e cuidados',
        'Aqui ficarão vacinas, consultas e procedimentos de cada animal.'
      ),
      emDesenvolvimento(
        'financeiro',
        'Financeiro',
        'Aqui ficarão doações, despesas e campanhas de arrecadação.'
      ),
      emDesenvolvimento(
        'voluntarios',
        'Voluntários',
        'Aqui ficarão o cadastro e a escala de voluntários.'
      ),
      emDesenvolvimento(
        'relatorios',
        'Relatórios',
        'Aqui ficarão relatórios para acompanhar o trabalho da ONG.'
      ),
      emDesenvolvimento(
        'configuracoes',
        'Configurações',
        'Aqui ficarão as configurações da organização.'
      ),
      { path: '**', redirectTo: 'visao-geral' }
    ]
  },
  {
    path: '**',
    redirectTo: ''
  }
];
