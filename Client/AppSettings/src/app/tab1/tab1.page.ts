import { Component } from '@angular/core';
import { PhotoService } from '../services/appSettings.service';
@Component({
  selector: 'app-tab1',
  templateUrl: 'tab1.page.html',
  styleUrls: ['tab1.page.scss']
})
export class Tab1Page {

  constructor(public photoService: PhotoService) {
    this.loadSavedImages();
  }

  public loadSavedImages() {

    var filter = this.photoService.GetTestFilter();
    this.photoService.LoadAppSettings(filter.ApplicationName, filter.RootKey, filter.RegionKey, filter.PropertyName);
    // this.photoService.LoadAppSettings("KeyA", "KeyB", "KeyC", "KeyD");
  }
}
