import Vue from 'vue';
import { Component } from 'vue-property-decorator';
 
interface wItem {
    Temp: number;
    Summary: string;
    City: string;
}
 
@Component
export default class WeatherComponent extends Vue {
    wItem: {} = { "temp": "", "summary": "", "city": ""};
 
    data() {
        return {
            wItem: { "temp": "", "summary": "", "city": ""}
        };
    }
    mounted() {
        fetch('/api/weather/London')
            .then(response => response.json() as Promise<wItem>)
            .then(data => {
                this.wItem = data;
            });
        console.log("mounted...")
    }
    beforeUpdate() {
        console.log("beforeUpdate...")
    }    
    updated() {
        console.log("updated...")
    }
}