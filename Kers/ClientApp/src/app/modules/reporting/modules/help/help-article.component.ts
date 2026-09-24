import { Component, Input, OnInit, ChangeDetectionStrategy } from '@angular/core';
import { Help } from '../admin/help/help.service';

@Component({
    selector: 'help-article',
    templateUrl: './help-article.component.html',
    styles: [],
    changeDetection: ChangeDetectionStrategy.Eager,
    standalone: false
})
export class HelpArticleComponent implements OnInit {
  @Input() article:Help;

  constructor() { }

  ngOnInit(): void {
  }

}
