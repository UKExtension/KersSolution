import { NgModule } from '@angular/core';
import { FroalaEditorDirective } from './froala-editor.directive';
import { FroalaViewDirective } from './froala-view.directive';

@NgModule({
  declarations: [FroalaEditorDirective, FroalaViewDirective],
  exports: [FroalaEditorDirective, FroalaViewDirective]
})
export class FroalaCompatModule { }