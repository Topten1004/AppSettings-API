import { Component } from '@angular/core';
import {Tab1Page} from '../tab1/tab1.page';

@Component({
  selector: 'app-tabs',
  templateUrl: 'tabs.page.html',
  styleUrls: ['tabs.page.scss']
})
export class TabsPage {

  constructor(public tab1Page : Tab1Page) {
    this.tab1Page = tab1Page;
  }


  public LoadPictures(){

    this.tab1Page.loadSavedImages();
  }
}


