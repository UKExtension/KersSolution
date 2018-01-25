import { Component, Input, Output, EventEmitter } from '@angular/core';
import {ExpenseService, Expense, ExpenseFundingSource, ExpenseMealRate, ExpenseMonth} from './expense.service';

@Component({
    selector: 'expense-month',
    templateUrl: 'expense-month.component.html'
})
export class ExpenseMonthComponent { 

    
    @Input() month:ExpenseMonth;
    @Output() onDeleted = new EventEmitter<Expense>();
    @Output() onEdited = new EventEmitter<Expense>();

    errorMessage: string;

    constructor( 
        private service:ExpenseService
    )   
    {}

    ngOnInit(){
       
       
       
    }

    deleted(expense:Expense){
        this.onDeleted.emit(expense);
    }
    edit(expense:Expense){
        this.onEdited.emit(expense)
    }

    
}