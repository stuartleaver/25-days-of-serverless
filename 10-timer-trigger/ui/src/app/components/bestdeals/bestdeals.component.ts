import { Component, OnInit } from '@angular/core';
import { BestdealsService } from 'src/app/services/bestdeals.service';

@Component({
  selector: 'app-bestdeals',
  templateUrl: './bestdeals.component.html',
  styleUrls: ['./bestdeals.component.css']
})
export class BestdealsComponent implements OnInit {
  bestdeals;

  constructor(private bestdealsService: BestdealsService) { }

  ngOnInit() {
    this.bestdeals = this.bestdealsService.getBestDeals();
    console.log(this.bestdeals)
  }

}
