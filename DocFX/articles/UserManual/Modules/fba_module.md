## Flux Balance Analysis (FBA)
Requires a [connection to Kosmogora](../Network/connecting_to_Kosmogora.md).

<img src="../../../resources/images/modules/fba.jpg" alt="FBA" style="height: 300px;"/>

This is the module to request a flux balance analysis for the model currently imported.

### The *Server Parameters* pannel:
<img src="../../../resources/images/modules/1x/fba_panel_1.png" alt="FBA Parameters" style="height: 300px;"/>

1. Input Field *Data Server IP*. The IP address at which *Kosmogora* is hosted. Default value is 127.0.0.1 but this only works during development if *Kosmogora* is hosted on the same machine as the one we are developping ECellDive on.
2. Input Field *Data Server Port*. The Port at which *Kosmogora* is listening. Default value is 8000. This is also the default value when launching *Kosmogora* so, unless you specified a port at that time, you can leave the field empty in ECellDive.
3. Button *Run FBA*. Will try to perform the FBA calculations while taking into account the reactions that may have been knocked out in the loaded model. It typically takes a few seconds before being able to see the result of the FBA. During this time, the module will be animated. If the query is successful, the module will flash <span style="color:green">*green*</span>; the size of the edges of the metabolic network will be updated. If the query fails, the module will flash <span style="color:red">*red*</span>. In that case, you can check the error message in the [log](/articles/UserManual/UIMenus/log_menu.html).

### The *FBA Visuals Parameters* pannel:

<img src="../../../resources/images/modules/1x/fba_panel_2.png" alt="FBA Parameters" style="height: 300px;"/>

1. Slider *Lower Bound*. The global value used to clamp the small flux values. If a reaction's flux value is lower than the lower bound, then the lower bound value will be used when updating the visuals of the reaction edge. This makes every reaction visually identical when their flux value is lower than the lower bound.
2. Color Button *Lower Bound*. The button used to set the color corresponding to the lower bound for the color gradient going from the lower bound to the upper bound.
3. Slider *Upper Bound*. The global value used to clamp the big flux values. If a reaction's flux value is greater than the upper bound, then the upper bound value will be used when updating the visuals of the reaction edge. This makes every reaction visually identical when their flux value is greater than the upper bound.
4. Color Button *Lower Bound*. The button used to set the color corresponding to the lower bound for the color gradient going from the lower bound to the upper bound.
5. Button *Update Visuals*. Updates the visuals of every reaction edge according to the setting that were just defined. This is local, there is no connection to *Kosmogora* involved so the update should be fast.