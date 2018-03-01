import Vue from 'vue';
import { Component } from 'vue-property-decorator';
 
interface wItem {
    Temp: number;
    Summary: string;
    City: string;
}
 
@Component
export default class WeatherComponent extends Vue {
    wItem: {} = { "temp": "", "summary": "", "city": "" }
    cities = ["London", "Montreal", "New York"]
    count: number = 0;

    data() {
        return {
            wItem: { "temp": "", "summary": "", "city": ""}
        };
    }
    mounted() {
        console.log("mounted...", this.count)
        console.log("value before...", this.cities.length, this.count)
        this.getData()
    }
    getData() {
        fetch('/api/weather/' + this.cities[this.count])
            .then(response => response.json() as Promise<wItem>)
            .then(data => {
                this.wItem = data;
            });
 //       this.count = (this.count >= this.cities.length - 1) ? 0 : this.count+1
        console.log("values after...", this.cities.length, this.count)
        
    }
    updateCount() {
        this.count = (this.count >= this.cities.length - 1) ? 0 : this.count+1
        this.getData()
    }
    beforeCreate() {
        console.log("beforeCreate...")
    }    
    created() {
        console.log("created...")
    }
    beforeUpdate() {
        console.log("beforeUpdate...")
    }    
    updated() {
        console.log("updated...")
    }
}