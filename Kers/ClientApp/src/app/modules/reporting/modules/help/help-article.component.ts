import { Component, Input, OnInit } from '@angular/core';
import { Help } from '../admin/help/help.service';

@Component({
  selector: 'help-article',
  templateUrl: './help-article.component.html',
  styles: [
  ]
})
export class HelpArticleComponent implements OnInit {
  @Input() article:Help;

  constructor() { }

  ngOnInit(): void {
  }

}
