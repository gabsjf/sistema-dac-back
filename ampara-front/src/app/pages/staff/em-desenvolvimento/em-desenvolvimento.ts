import { Component, inject } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';

// Áreas do menu que ainda não têm funcionalidade nem API correspondente.
@Component({
  selector: 'app-em-desenvolvimento',
  imports: [RouterLink],
  templateUrl: './em-desenvolvimento.html',
  styleUrl: './em-desenvolvimento.css'
})
export class EmDesenvolvimento {
  private readonly dados = inject(ActivatedRoute).snapshot.data;

  readonly secao: string = this.dados['secao'] ?? 'Esta área';
  readonly descricao: string = this.dados['descricao'] ?? '';
}
