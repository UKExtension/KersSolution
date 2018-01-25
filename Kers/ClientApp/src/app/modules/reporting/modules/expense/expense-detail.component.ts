import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from './expense.service';

@Component({
    selector: 'expense-detail',
    templateUrl: 'expense-detail.component.html'
})
export class ExpenseDetailComponent { 
    rowDefault =true;
    rowEdit = false;
    rowDelete = false;
    
    @Input() expense:Expense;

    @Output() onDeleted = new EventEmitter<Expense>();
    @Output() onEdited = new EventEmitter<Expense>();
    
    errorMessage: string;

    constructor( 
        private service:ExpenseService
    )   
    {}

    ngOnInit(){
       
       
       
    }
    edit(){
        this.rowDefault = false;
        this.rowEdit = true;
        this.rowDelete = false;
    }
    delete(){
        this.rowDefault = false;
        this.rowEdit = false;
        this.rowDelete = true;
    }
    default(){
        this.rowDefault = true;
        this.rowEdit = false;
        this.rowDelete = false;
    }

    expenseSubmitted(expense:Expense){
        this.expense = expense;
        this.onEdited.emit(expense);
        this.default();
    }

    confirmDelete(){
        
        this.service.delete(this.expense.id).subscribe(
            res=>{
                this.onDeleted.emit(this.expense);
            },
            err => this.errorMessage = <any> err
        );
        
    }
    

    
}