import { Component, Input } from '@angular/core';
import {User} from '../../../modules/user/user.service';



@Component({
    selector: 'widget-my-info',
    template: `
<div class="col-md-6 col-xs-12" *ngIf="user">
    <div class="x_panel">
        <div class="x_title">
            <h2>Info</h2>
            <div class="clearfix"></div>
        </div>
        <div class="x_content">
            <div>
                Planning Unit Name: <strong>{{user.rprtngProfile.planningUnit.name}}</strong><br>
                Position:  <strong>{{user.extensionPosition.title}}</strong><br>
                <br><br><br>
            </div>

            <p>For questions or assistance, please email:<br><a href="mailto:KERS-HELP@LSV.UKY.EDU">KERS-HELP@LSV.UKY.EDU</a></p>


         </div>
    </div>
</div>
  `
})
export class WidgetMyInfoComponent { 
    @Input() user:User;

}