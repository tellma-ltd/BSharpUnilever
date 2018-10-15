import { Component, OnInit, TemplateRef, Input } from '@angular/core';

@Component({
  selector: 'b-master',
  templateUrl: './master.component.html',
  styleUrls: ['./master.component.scss']
})
export class MasterComponent implements OnInit {

  @Input()
  controller: string;

  @Input()
  public tileTemplate: TemplateRef<any>;

  @Input()
  public tableRowTemplate: TemplateRef<any>;

  @Input()
  public gridDefinition: { display: string, orderBy?: string }[];


  public search: string;
  private isTiles = true; // TODO remove


  constructor() { }

  ngOnInit() {
  }

  get from(): number {
    return 51;
  }

  get to(): number {
    return 100;
  }

  get total(): number {
    return 1520;
  }

  onFirstPage() {

  }

  get canFirstPage(): boolean {
    return true;
  }

  onPreviousPage() {

  }

  get canPreviousPage(): boolean {
    return true;
  }

  onNextPage() {

  }

  get canNextPage(): boolean {
    return true;
  }

  get showTilesView(): boolean {
    return this.isTiles;
  }

  get showTableView(): boolean {
    return !this.isTiles;
  }

  onTilesView() {
    this.isTiles = true;
  }

  onTableView() {
    this.isTiles = false;
  }

  isRecentlyOpened(entity: any) {
    return false;
  }
}
