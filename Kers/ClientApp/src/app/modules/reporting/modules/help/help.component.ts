import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { HelpCategory, HelpService } from '../admin/help/help.service';

@Component({
  selector: 'app-help',
  templateUrl: './help.component.html',
  styleUrls: ['./help.component.scss']
})
export class HelpComponent implements OnInit {
  categories:Observable<HelpCategory[]>;

  constructor(
    private service:HelpService
  ) { 
    
  }

  ngOnInit(): void {
    this.categories = this.service.allCategories();
  }

}
