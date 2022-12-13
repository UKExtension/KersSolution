import { Component, Input, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { Help, HelpCategory, HelpService } from '../admin/help/help.service';

@Component({
  selector: 'help-category',
  templateUrl: './help-category.component.html',
  styles: [
  ]
})
export class HelpCategoryComponent implements OnInit {
  @Input() parentId:number;
  @Input() dataList:HelpCategory[];

  constructor(
    private service:HelpService
  ) { }

  ngOnInit(): void {
  }

  removeCurrentLevelItems=(datalist:HelpCategory[],parentId:number)=>{
    //logic here to remove current level items
    return datalist.filter(item=>item.parentId != parentId)
    // return datalist;
  }
  articlesByCategory(categoryId:number):Observable<Help[]>{
    var helps = this.service.bayCategory(categoryId);
    return helps;
  }

}
