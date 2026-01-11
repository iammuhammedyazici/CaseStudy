(function () {
    const originalFetch = window.fetch;
    window.fetch = async function (...args) {
        const response = await originalFetch(...args);
        const url = args[0] instanceof Request ? args[0].url : args[0];

        if (url && url.endsWith("/dev/generate-token") && response.ok) {
            const clone = response.clone();
            clone.json().then(data => {
                if (data.token) {
                    const token = data.token;
                    const auth = {
                        "Bearer": {
                            "schema": {
                                "type": "http",
                                "scheme": "bearer",
                                "bearerFormat": "JWT"
                            },
                            "clientId": "client1",
                            "name": "Bearer",
                            "value": token
                        }
                    };

                    if (window.ui) {
                        window.ui.authActions.authorize(auth);
                        console.log("Auto-authenticated with generated token!");
                    }
                }
            }).catch(err => console.error("Auto-auth failed:", err));
        }
        return response;
    };
})();
