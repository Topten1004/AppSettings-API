import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import {PhotoService} from '../services/appSettings.service';

@Component({
  selector: 'app-tab2',
  templateUrl: 'tab2.page.html',
  styleUrls: ['tab2.page.scss']
})
export class Tab2Page {

  constructor(public photoService : PhotoService) {

  }

  addPhotoToGallery(){

    this.photoService.addNewToGallery();
  
  }

  

}
