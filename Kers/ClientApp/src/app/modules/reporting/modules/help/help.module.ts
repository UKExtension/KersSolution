import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { HelpRoutingModule } from './help-routing.module';
import { HelpComponent } from './help.component';
import { HelpCategoryComponent } from './help-category.component';
import { HelpArticleComponent } from './help-article.component';


@NgModule({
  declarations: [
    HelpComponent,
    HelpCategoryComponent,
    HelpArticleComponent
  ],
  imports: [
    CommonModule,
    HelpRoutingModule
  ]
})
export class HelpModule { }
