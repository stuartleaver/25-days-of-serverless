const LOCAL_BASE_URL = 'http://localhost:7071';
const AZURE_BASE_URL = 'https://santasreindeerguidancedeliverysystem.azurewebsites.net';

const getAPIBaseUrl = () => {
    const isLocal = /localhost/.test(window.location.href);
    return isLocal ? LOCAL_BASE_URL : AZURE_BASE_URL;
}

const app = new Vue({
    el: '#app',
    data() { 
        return {
            systems: []
        }
    },
    methods: {
        async getStatus() {
            try {
                const apiUrl = `${getAPIBaseUrl()}/api/getStatus`;
                const response = await axios.get(apiUrl);
                console.log('Status fetched from ', apiUrl);
                app.systems = response.data;
            } catch (ex) {
                console.error(ex);
            }
        }
    },
    created() {
        this.getStatus();
    }
});

const connect = () => {
    const connection = new signalR.HubConnectionBuilder().withUrl(`${getAPIBaseUrl()}/api`).build();

    connection.onclose(()  => {
        console.log('SignalR connection disconnected');
        setTimeout(() => connect(), 2000);
    });

    connection.on('updated', updatesSystem => {
        const index = app.systems.findIndex(s => s.id === updatesSystem.id);
        app.systems.splice(index, 1, updatesSystem);
    });

    connection.start().then(() => {
        console.log("SignalR connection established");
    });
};

connect();