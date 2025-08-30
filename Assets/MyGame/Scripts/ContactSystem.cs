using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Obvious.Soap;

namespace BOH
{
    public class ContactSystem : MonoBehaviour
    {
        [System.Serializable]
        public class ContactData
        {
            public ContactSO contact;
            public int currentTrust = 0;
            public int currentBond = 0;
        }

        [Header("Contacts")]
        [SerializeField] private List<ContactData> contacts = new List<ContactData>();
        
        [Header("Events")]
        [SerializeField] private ScriptableEventInt onTrustChanged;
        [SerializeField] private ScriptableEventNoParam onBondMilestone;

        private void Start()
        {
            // Initialize trust levels
            foreach (var contact in contacts)
            {
                contact.currentTrust = contact.contact.startingTrust;
            }
        }

        public void ModifyTrust(string contactId, int delta)
        {
            var contactData = contacts.FirstOrDefault(c => c.contact.contactId == contactId);
            if (contactData == null) return;
            
            int oldTrust = contactData.currentTrust;
            contactData.currentTrust = Mathf.Clamp(
                contactData.currentTrust + delta, 
                0, 
                contactData.contact.maxTrust
            );
            
            if (oldTrust != contactData.currentTrust)
            {
                Debug.Log($"{contactId} trust changed: {oldTrust} -> {contactData.currentTrust}");
                onTrustChanged?.Raise(contactData.currentTrust);
            }
        }

        public void ModifyBond(string contactId, int delta)
        {
            var contactData = contacts.FirstOrDefault(c => c.contact.contactId == contactId);
            if (contactData == null || !contactData.contact.isBondProspect) return;
            
            int oldBond = contactData.currentBond;
            contactData.currentBond += delta;
            
            // Check for bond milestones
            if ((oldBond < 5 && contactData.currentBond >= 5) ||
                (oldBond < 10 && contactData.currentBond >= 10))
            {
                Debug.Log($"Bond milestone reached with {contactId}: {contactData.currentBond}");
                onBondMilestone?.Raise();
            }
        }

        public int GetTrust(string contactId)
        {
            var contactData = contacts.FirstOrDefault(c => c.contact.contactId == contactId);
            return contactData?.currentTrust ?? 0;
        }

        public ContactSO GetContact(string contactId)
        {
            var contactData = contacts.FirstOrDefault(c => c.contact.contactId == contactId);
            return contactData?.contact;
        }
    }

    // ScriptRole: Manages NPC trust and bond relationships
    // RelatedScripts: GiftingSystem, DialogueSystem
    // UsesSO: ContactSO, IntEventSO, GameEventSO
    // ReceivesFrom: GiftingSystem, ErrandSystem
    // SendsTo: UI, JournalSystem
}