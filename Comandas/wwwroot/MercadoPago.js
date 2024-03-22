window.updateAmount = function (amount, parcelas, paymentMethods) {
    const mp = new MercadoPago('APP_USR-513d7623-2167-4134-9ebf-a78af2cb8047', {
        locale: 'pt-BR'
    });
    const bricksBuilder = mp.bricks();
    const renderPaymentBrick = async (bricksBuilder) => {
        const paymentMethodsSetting = paymentMethods ?
            {
                creditCard: "all",
                debitCard: "all",
                bankTransfer: "all",
                maxInstallments: parcelas
            } :
            {
                creditCard: "all",
                maxInstallments: parcelas
            };

        const settings = {
            initialization: {
                amount: amount,
                preferenceId: "145263082-debbe8df-8167-4370-9b66-5cb4de706a7c",
            },
            customization: {
                paymentMethods: paymentMethodsSetting
            },
            callbacks: {
                onReady: () => {
                    // Callback chamado quando o Brick está pronto.
                    // Aqui, você pode ocultar seu site, por exemplo.
                },
                onSubmit: ({ selectedPaymentMethod, formData }) => {
                    // callback chamado quando há click no botão de envio de dados
                    return new Promise((resolve, reject) => {
                        fetch("api/pagamentos", {
                            method: "POST",
                            headers: {
                                "Content-Type": "application/json",
                            },
                            body: JSON.stringify(formData),
                        })
                            .then((response) => response.json())
                            .then((response) => {
                                const renderStatusScreenBrick = async (bricksBuilder) => {
                                    const settings = {
                                        initialization: {
                                            paymentId: response.id,
                                        },
                                        callbacks: {
                                            onReady: () => {
                                                // Callback chamado quando o Brick estiver pronto.
                                                // Aqui você pode ocultar loadings do seu site, por exemplo.
                                            },
                                            onError: (error) => {
                                                // callback chamado para todos os casos de erro do Brick
                                                console.error(error);
                                            },
                                        },
                                    };
                                    window.statusScreenBrickController = await bricksBuilder.create(
                                        'statusScreen',
                                        'statusScreenBrick_container',
                                        settings,
                                    );
                                };
                                renderStatusScreenBrick(bricksBuilder);
                                resolve();
                            })
                            .catch((error) => {
                                // manejar a resposta de erro ao tentar criar um pagamento
                                reject();
                            });
                    });
                },
                onError: (error) => {
                    // callback chamado para todos os casos de erro do Brick
                    console.error(error);
                },
            },
        };
        window.paymentBrickController = await bricksBuilder.create(
            "payment",
            "paymentBrick_container",
            settings
        );
    };
    renderPaymentBrick(bricksBuilder);
}
