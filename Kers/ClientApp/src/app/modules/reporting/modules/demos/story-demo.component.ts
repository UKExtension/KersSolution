import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Params } from '@angular/router';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-story-demo',
  template: `
  
    <story-form-demo [help_sections]="help_sections"></story-form-demo>

  `,
  styles: [
  ]
})
export class StoryDemoComponent implements OnInit {
  public help_sections = "1";
  loading = true;

  constructor(
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {



    this.help_sections = this.route.snapshot.paramMap.get('id') ?? "1";
    console.log(this.help_sections);
    /* 
    this.route.params.subscribe(
      (params: Params) => {
        console.log(params);
          if(params["id"] != undefined){
            this.help_sections = params["id"]
          }
          this.loading = false;
          return null
        }
    )
     */
    

  }

}
