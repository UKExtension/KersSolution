import { Component, Input, Output, EventEmitter } from '@angular/core';
import * as Survey from 'survey-angular';

@Component({
    selector: 'survey',
    template: `<div class="survey-container contentcontainer codecontainer"><div id="surveyElement"></div></div>`,
})
export class SurveyComponent  {
    @Output() submitSurvey = new EventEmitter<any>();
    @Input() json: any;
    result: any;

    ngOnInit() {
        let surveyModel = new Survey.Model(this.json);

        surveyModel.onAfterRenderQuestion.add((survey, options) => {
            if (!options.question.popupdescription) { return; }
            // Add a button;
            const btn = document.createElement('button');
            btn.className = 'btn btn-info btn-xs';
            btn.innerHTML = 'More Info';
            btn.onclick = function () {
              // showDescription(question);
              alert(options.question.popupdescription);
            };
            const header = options.htmlElement.querySelector('h5');
            const span = document.createElement('span');
            span.innerHTML = '  ';
            header.appendChild(span);
            header.appendChild(btn);
          });
          surveyModel.onComplete
            .add((result, options) => {
              this.submitSurvey.emit(result.data);
              this.result = result.data;
            }
            );



        Survey.SurveyNG.render('surveyElement', { model: surveyModel });
    }
}