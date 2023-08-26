using UnityEngine.XR.Interaction.Toolkit;

public class XrTriggerWithoutGrab : XRSimpleInteractable
{
    protected override void OnHoverEntered(HoverEnterEventArgs args)
    {
        base.OnHoverEntered(args);
        // Mark this object as selected
        (args.interactorObject as XRBaseInteractor).StartManualInteraction(this as IXRSelectInteractable);
    }

    protected override void OnHoverExited(HoverExitEventArgs args)
    {
        base.OnHoverExited(args);
        // Mark this object as unselected
        (args.interactorObject as XRBaseInteractor).EndManualInteraction();
    }
}