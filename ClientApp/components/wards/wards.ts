import Vue from 'vue';
import { Component } from 'vue-property-decorator';
 
interface WardItem {
    Id: number;
    Name: string;
}
 
@Component
export default class WardsComponent extends Vue {
  wards: WardItem[] = [];

  data() {
    return {
        wards: []
    };
}
    mounted() {
        this.getData()
    }
    getData() {
        fetch('/api/ward/')
            .then(response => response.json() as Promise<WardItem[]>)
            .then(data => {
                this.wards = data;
            });
        
    }
    update() {
        this.getData()
    }
}