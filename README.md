# Device.Gate
Takes in the messages from the node, dismisses all else

For now, this process allows the rabbit messages and filters them, only passing
the ones with valid serial numbers (any of the correct length and character) so
that the systems behind it aren't flooded.  

In the future this will take in MQTT messages and translate them, send them off
to their next destination by type.
