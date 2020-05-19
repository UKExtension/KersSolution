import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { UserService, User } from '../user/user.service';
import { LadderService } from './ladder.service';
import { Observable } from 'rxjs';
import { LadderApplication, LadderStage } from './ladder';
import { switchMap } from 'rxjs/operators';
import { ReportingService } from '../../components/reporting/reporting.service';

@Component({
  selector: 'ladder-review',
  templateUrl: './ladder-review.component.html',
  styles: []
})
export class LadderReviewComponent implements OnInit {

  aplications$:Observable<LadderApplication>;
  user:User;
  loading = true;

  constructor(
    private route: ActivatedRoute,
    private userService:UserService,
    private service: LadderService,
    private router:Router,
    private reportingService: ReportingService
  ) { }

  ngOnInit() {
    this.route.paramMap.pipe(
      switchMap((params: ParamMap) =>{
          return this.service.getStage(+params.get('stageId'));
        }
      )
    ).subscribe(
      stage => {
        var st:LadderStage = stage;
        var rlCodes: string[] = stage.ladderStageRoles.map( r => r.zEmpRoleType.shortTitle);
        this.userService.current().subscribe(
          res => {
            this.user = res;
            this.userService.currentUserHasAnyOfTheRoles(rlCodes).subscribe(
              res => {
                if( !res ){
                  // display message and redirect
                  this.reportingService.setAlert("Your are not authorized to reach this page.");
                  this.router.navigate(['']);
                }else{
                  // Load applications
                  this.service.getApplicationsForReview(stage.id).subscribe(
                    apps => {
                      console.log( apps );
                      this.loading = false;
                    }
                  )

                }
              }
            )
          }
        )
        
      }
    );
     
    /* 
    
    this.aplications$ = this.route.paramMap.pipe(
      switchMap((params: ParamMap) =>{

          return this.service.getTraining(+params.get('stageId'));
        }
      )
    );
    
     */
    
    



  }

  defaultTitle(){
    this.reportingService.setTitle("Professional Promotion Application");
    this.reportingService.setSubtitle("Review");
  }

}
