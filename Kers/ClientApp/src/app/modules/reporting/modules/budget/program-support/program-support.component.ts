import { Component, OnInit, ViewEncapsulation } from '@angular/core';

@Component({
  standalone: false,
  selector: 'program-support',
  templateUrl: './program-support.component.html',
  styleUrls: ['./program-support.component.scss'],
  encapsulation: ViewEncapsulation.None
})
export class ProgramSupportComponent implements OnInit {

  tabs = ['Program Support Overview', 'Individual Budgets', 'Reconciliation', 'Signatures', 'Reports'];
  activeTab = 0;

  summary = [
    { icon: 'fa-database', tone: 'green', amount: '$21,000', label: 'Total Budgeted', note: 'From Budget Plan' },
    { icon: 'fa-user', tone: 'blue', amount: '$21,000', label: 'Total Assigned', note: '100% of budgeted' },
    { icon: 'fa-file-text', tone: 'purple', amount: '$18,000', label: 'Total Detailed', note: '86% of assigned' },
    { icon: 'fa-exclamation-triangle', tone: 'red', amount: '$3,000', label: 'Undetailed Variance', note: '14% of assigned' }
  ];

  categories = [
    { name: 'Demonstration Supplies', amount: '$8,400', percent: 40, color: '#2f80ed' },
    { name: 'Equipment', amount: '$5,250', percent: 25, color: '#27ae60' },
    { name: 'Reference Materials', amount: '$3,150', percent: 15, color: '#f5a623' },
    { name: 'Teaching Aids', amount: '$2,100', percent: 10, color: '#8e5cf0' },
    { name: 'Other', amount: '$2,100', percent: 10, color: '#e74c3c' }
  ];

  donut = 'conic-gradient(#2f80ed 0 40%, #27ae60 40% 65%, #f5a623 65% 80%, #8e5cf0 80% 90%, #e74c3c 90% 100%)';

  positions = [
    { name: 'ANR Agent', percent: 100, count: '1/1', tone: 'success' },
    { name: 'FCS Agent', percent: 100, count: '1/1', tone: 'success' },
    { name: '4-H Agent', percent: 100, count: '1/1', tone: 'success' },
    { name: 'Program Assistant', percent: 50, count: '1/2', tone: 'warning' },
    { name: 'NEP', percent: 0, count: '0/1', tone: 'danger' },
    { name: 'Other', percent: 0, count: '0/1', tone: 'muted' }
  ];

  budgets = [
    { id: '12345', name: 'ANR Agent', area: 'Agriculture & Natural Resources', budget: '$4,500', assigned: '$4,500', detailed: '$4,500', variance: '$0', status: 'Complete', tone: 'success' },
    { id: '23456', name: 'FCS Agent', area: 'Family & Consumer Sciences', budget: '$4,500', assigned: '$4,500', detailed: '$4,500', variance: '$0', status: 'Complete', tone: 'success' },
    { id: '34567', name: '4-H Agent', area: '4-H Youth Development', budget: '$4,500', assigned: '$4,500', detailed: '$4,000', variance: '($500)', status: 'In Progress', tone: 'info' },
    { id: '45678', name: 'Program Assistant', area: 'Support Staff', budget: '$2,000', assigned: '$2,000', detailed: '$1,000', variance: '($1,000)', status: 'Missing Details', tone: 'warning' },
    { id: '56789', name: 'NEP', area: 'Nutrition Education', budget: '$1,000', assigned: '$1,000', detailed: '$0', variance: '($1,000)', status: 'Not Started', tone: 'danger' },
    { id: '67890', name: 'Other', area: 'Other Programs', budget: '$4,500', assigned: '$4,500', detailed: '$4,000', variance: '($500)', status: 'In Progress', tone: 'info' }
  ];

  detailItems = [
    { category: 'Demonstration Supplies', description: 'Educational supplies', amount: '$1,500' },
    { category: 'Equipment', description: 'Field equipment', amount: '$1,000' },
    { category: 'Reference Materials', description: 'Curricula and resources', amount: '$750' },
    { category: 'Teaching Aids', description: 'Visual aids / displays', amount: '$500' },
    { category: 'Other', description: 'Program materials', amount: '$250' }
  ];

  validations = [
    'Program support total matches Budget Plan',
    'Assigned amounts match personnel allocations',
    'All detailed budgets balance to assigned amounts',
    'Required signatures collected (0 remaining)'
  ];

  quickActions = [
    'Export All Budgets (Excel)',
    'Generate Individual Budget Forms (PDF)',
    'Program Support Summary Report',
    'Check Reconciliation',
    'View Signature Status'
  ];

  constructor() { }

  ngOnInit() {
  }

}
